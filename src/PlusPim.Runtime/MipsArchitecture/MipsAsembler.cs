using PlusPim.Runtime.Services;
using System.Text.RegularExpressions;

namespace PlusPim.Runtime.MipsArchitecture;

internal partial class MipsAsembler: IAssembler {
    private const int _initialAddress = 0x00400000;
    private const char _commentChar = '#'; // MIPSの標準コメント文字

    [GeneratedRegex(@"^([a-zA-Z_][a-zA-Z0-9_]*):$", RegexOptions.Compiled)]
    private static partial Regex _labelRegex();

    private readonly Dictionary<string, uint> _symbolTable = [];

    /// <summary>
    /// ラベルとアドレスのシンボルテーブルを作成する
    /// </summary>
    /// <param name="lines">コメント・空行削除済みの行</param>
    private void MakeSymbolTable(string[] lines) {
        this._symbolTable.Clear();
        uint currentAddress = _initialAddress;

        foreach(string line in lines) {
            // ラベルかチェック
            Match labelMatch = _labelRegex().Match(line);
            if(labelMatch.Success) {
                string labelName = labelMatch.Groups[1].Value;
                this._symbolTable[labelName] = currentAddress;
                continue;
            }

            // 命令行ならアドレスを進める
            if(!string.IsNullOrWhiteSpace(line)) {
                currentAddress += 4;
            }
        }
    }

    /// <summary>
    /// 命令をアセンブルする
    /// </summary>
    /// <param name="lines">コメント・空行削除済みの行</param>
    /// <returns>アセンブル済みのバイナリデータ</returns>
    private byte[] AssembleInstructions(string[] lines) {
        List<byte> machineCode = [];
        uint currentAddress = _initialAddress;

        foreach(string line in lines) {
            // ラベル行はスキップ
            // ラベルを除去しておけば重い正規表現を使わなくてもよいかも
            if(_labelRegex().IsMatch(line)) {
                continue;
            }

            // 命令をパースしてエンコード
            uint? instruction = this.ParseAndEncodeInstruction(line, currentAddress);
            if(instruction.HasValue) {
                byte[] bytes = MipsInstructionEncoder.ToBytes(instruction.Value);
                machineCode.AddRange(bytes);
                currentAddress += 4;
            }
        }

        return [.. machineCode];
    }

    /// <summary>
    /// 命令文字列をパースしてバイナリにエンコードする
    /// </summary>
    /// <param name="line">命令行</param>
    /// <param name="currentAddress">現在のアドレス</param>
    /// <returns>エンコードされた32ビット命令、またはnull</returns>
    private uint? ParseAndEncodeInstruction(string line, uint currentAddress) {
        // 命令とオペランドに分割
        string[] parts = line.Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries);
        if(parts.Length == 0) {
            return null;
        }

        string mnemonic = parts[0].ToLower();
        string[] operands = parts.Length > 1
            ? parts[1].Split(',', StringSplitOptions.TrimEntries)
            : [];

        // R-type命令の処理
        if(EnumExtensions.TryParseByDescription<MipsRtypeFunction>(mnemonic, out MipsRtypeFunction rtypeFunc)) {
            return this.EncodeRTypeInstruction(rtypeFunc, operands);
        }

        // I-type, J-type命令の処理
        return EnumExtensions.TryParseByDescription<MipsOpcode>(mnemonic, out MipsOpcode opcode)
            ? this.EncodeNonRTypeInstruction(opcode, operands, currentAddress)
            : throw new InvalidOperationException($"Unknown instruction: {mnemonic}");
    }

    /// <summary>
    /// R-type命令をエンコード
    /// </summary>
    private uint? EncodeRTypeInstruction(MipsRtypeFunction funct, string[] operands) {
        // 命令の種類によってオペランドのフォーマットが異なる
        return funct switch {
            // Shift operations: sll $rd, $rt, shamt
            MipsRtypeFunction.Sll or MipsRtypeFunction.Srl or MipsRtypeFunction.Sra => this.EncodeShiftImmediate(funct, operands),

            // Shift variable: sllv $rd, $rt, $rs
            MipsRtypeFunction.Sllv or MipsRtypeFunction.Srlv or MipsRtypeFunction.Srav => this.EncodeShiftVariable(funct, operands),

            // Jump register: jr $rs
            MipsRtypeFunction.Jr => this.EncodeJumpRegister(funct, operands),

            // Jump and link register: jalr $rd, $rs (or jalr $rs with implicit $rd=$ra)
            MipsRtypeFunction.Jalr => this.EncodeJumpAndLinkRegister(funct, operands),

            // Move from HI/LO: mfhi $rd, mflo $rd
            MipsRtypeFunction.Mfhi or MipsRtypeFunction.Mflo => this.EncodeMoveFrom(funct, operands),

            // Multiply/Divide: mult $rs, $rt
            MipsRtypeFunction.Mult or MipsRtypeFunction.Multu or MipsRtypeFunction.Div or MipsRtypeFunction.Divu => this.EncodeMultDiv(funct, operands),

            // Arithmetic/Logical: add $rd, $rs, $rt
            _ => this.EncodeThreeRegister(funct, operands)
        };
    }

    /// <summary>
    /// 3レジスタ形式のR-type命令: op $rd, $rs, $rt
    /// </summary>
    private uint EncodeThreeRegister(MipsRtypeFunction funct, string[] operands) {
        if(operands.Length != 3) {
            throw new ArgumentException($"Expected 3 operands for {funct}");
        }

        int rd = MipsRegisterParser.Parse(operands[0]);
        int rs = MipsRegisterParser.Parse(operands[1]);
        int rt = MipsRegisterParser.Parse(operands[2]);

        return MipsInstructionEncoder.EncodeRType(funct, rs, rt, rd);
    }

    /// <summary>
    /// シフト即値形式: sll $rd, $rt, shamt
    /// </summary>
    private uint EncodeShiftImmediate(MipsRtypeFunction funct, string[] operands) {
        if(operands.Length != 3) {
            throw new ArgumentException($"Expected 3 operands for {funct}");
        }

        int rd = MipsRegisterParser.Parse(operands[0]);
        int rt = MipsRegisterParser.Parse(operands[1]);
        int shamt = this.ParseImmediate(operands[2]);

        return MipsInstructionEncoder.EncodeRType(funct, 0, rt, rd, shamt);
    }

    /// <summary>
    /// シフト可変形式: sllv $rd, $rt, $rs
    /// </summary>
    private uint EncodeShiftVariable(MipsRtypeFunction funct, string[] operands) {
        if(operands.Length != 3) {
            throw new ArgumentException($"Expected 3 operands for {funct}");
        }

        int rd = MipsRegisterParser.Parse(operands[0]);
        int rt = MipsRegisterParser.Parse(operands[1]);
        int rs = MipsRegisterParser.Parse(operands[2]);

        return MipsInstructionEncoder.EncodeRType(funct, rs, rt, rd);
    }

    /// <summary>
    /// ジャンプレジスタ形式: jr $rs
    /// </summary>
    private uint EncodeJumpRegister(MipsRtypeFunction funct, string[] operands) {
        if(operands.Length != 1) {
            throw new ArgumentException($"Expected 1 operand for {funct}");
        }

        int rs = MipsRegisterParser.Parse(operands[0]);
        return MipsInstructionEncoder.EncodeRType(funct, rs, 0, 0);
    }

    /// <summary>
    /// ジャンプアンドリンクレジスタ形式: jalr $rd, $rs
    /// </summary>
    private uint EncodeJumpAndLinkRegister(MipsRtypeFunction funct, string[] operands) {
        int rd = 31; // デフォルトは$ra
        int rs;

        if(operands.Length == 1) {
            rs = MipsRegisterParser.Parse(operands[0]);
        } else if(operands.Length == 2) {
            rd = MipsRegisterParser.Parse(operands[0]);
            rs = MipsRegisterParser.Parse(operands[1]);
        } else {
            throw new ArgumentException($"Expected 1 or 2 operands for {funct}");
        }

        return MipsInstructionEncoder.EncodeRType(funct, rs, 0, rd);
    }

    /// <summary>
    /// Move from HI/LO: mfhi $rd
    /// </summary>
    private uint EncodeMoveFrom(MipsRtypeFunction funct, string[] operands) {
        if(operands.Length != 1) {
            throw new ArgumentException($"Expected 1 operand for {funct}");
        }

        int rd = MipsRegisterParser.Parse(operands[0]);
        return MipsInstructionEncoder.EncodeRType(funct, 0, 0, rd);
    }

    /// <summary>
    /// Multiply/Divide: mult $rs, $rt
    /// </summary>
    private uint EncodeMultDiv(MipsRtypeFunction funct, string[] operands) {
        if(operands.Length != 2) {
            throw new ArgumentException($"Expected 2 operands for {funct}");
        }

        int rs = MipsRegisterParser.Parse(operands[0]);
        int rt = MipsRegisterParser.Parse(operands[1]);

        return MipsInstructionEncoder.EncodeRType(funct, rs, rt, 0);
    }

    /// <summary>
    /// I-type, J-type命令をエンコード
    /// </summary>
    private uint? EncodeNonRTypeInstruction(MipsOpcode opcode, string[] operands, uint currentAddress) {
        return opcode switch {
            // J-type: j target, jal target
            MipsOpcode.J or MipsOpcode.Jal => this.EncodeJump(opcode, operands),

            // Branch: beq $rs, $rt, label
            MipsOpcode.Beq or MipsOpcode.Bne => this.EncodeBranch(opcode, operands, currentAddress),

            // Branch zero: blez $rs, label
            MipsOpcode.Blez or MipsOpcode.Bgtz => this.EncodeBranchZero(opcode, operands, currentAddress),

            // Load/Store: lw $rt, offset($rs)
            MipsOpcode.Lw or MipsOpcode.Lh or MipsOpcode.Lb or MipsOpcode.Lbu or MipsOpcode.Lhu or
            MipsOpcode.Sw or MipsOpcode.Sh or MipsOpcode.Sb => this.EncodeLoadStore(opcode, operands),

            // Immediate arithmetic: addi $rt, $rs, imm
            MipsOpcode.Addi or MipsOpcode.Addiu or MipsOpcode.Slti or MipsOpcode.Sltiu or
            MipsOpcode.Andi or MipsOpcode.Ori or MipsOpcode.Xori => this.EncodeImmediate(opcode, operands),

            // Load Upper Immediate: lui $rt, imm
            MipsOpcode.Lui => this.EncodeLui(opcode, operands),

            _ => throw new InvalidOperationException($"Unsupported opcode: {opcode}")
        };
    }

    /// <summary>
    /// J-type命令: j label, jal label
    /// </summary>
    private uint EncodeJump(MipsOpcode opcode, string[] operands) {
        if(operands.Length != 1) {
            throw new ArgumentException($"Expected 1 operand for {opcode}");
        }

        uint targetAddress = this.ResolveLabel(operands[0]);
        uint wordAddress = targetAddress >> 2; // アドレスを4で割る（ワードアドレス化）

        return MipsInstructionEncoder.EncodeJType(opcode, wordAddress);
    }

    /// <summary>
    /// 分岐命令: beq $rs, $rt, label
    /// </summary>
    private uint EncodeBranch(MipsOpcode opcode, string[] operands, uint currentAddress) {
        if(operands.Length != 3) {
            throw new ArgumentException($"Expected 3 operands for {opcode}");
        }

        int rs = MipsRegisterParser.Parse(operands[0]);
        int rt = MipsRegisterParser.Parse(operands[1]);
        short offset = this.CalculateBranchOffset(operands[2], currentAddress);

        return MipsInstructionEncoder.EncodeIType(opcode, rs, rt, offset);
    }

    /// <summary>
    /// ゼロ分岐命令: blez $rs, label
    /// </summary>
    private uint EncodeBranchZero(MipsOpcode opcode, string[] operands, uint currentAddress) {
        if(operands.Length != 2) {
            throw new ArgumentException($"Expected 2 operands for {opcode}");
        }

        int rs = MipsRegisterParser.Parse(operands[0]);
        short offset = this.CalculateBranchOffset(operands[1], currentAddress);

        return MipsInstructionEncoder.EncodeIType(opcode, rs, 0, offset);
    }

    /// <summary>
    /// Load/Store命令: lw $rt, offset($rs)
    /// </summary>
    private uint EncodeLoadStore(MipsOpcode opcode, string[] operands) {
        if(operands.Length != 2) {
            throw new ArgumentException($"Expected 2 operands for {opcode}");
        }

        int rt = MipsRegisterParser.Parse(operands[0]);

        // offset($rs)をパース
        Match match = Regex.Match(operands[1], @"^(-?\d+)\((\$\w+)\)$");
        if(!match.Success) {
            throw new ArgumentException($"Invalid load/store format: {operands[1]}");
        }

        short offset = short.Parse(match.Groups[1].Value);
        int rs = MipsRegisterParser.Parse(match.Groups[2].Value);

        return MipsInstructionEncoder.EncodeIType(opcode, rs, rt, offset);
    }

    /// <summary>
    /// 即値演算命令: addi $rt, $rs, imm
    /// </summary>
    private uint EncodeImmediate(MipsOpcode opcode, string[] operands) {
        if(operands.Length != 3) {
            throw new ArgumentException($"Expected 3 operands for {opcode}");
        }

        int rt = MipsRegisterParser.Parse(operands[0]);
        int rs = MipsRegisterParser.Parse(operands[1]);
        short immediate = (short)this.ParseImmediate(operands[2]);

        return MipsInstructionEncoder.EncodeIType(opcode, rs, rt, immediate);
    }

    /// <summary>
    /// LUI命令: lui $rt, imm
    /// </summary>
    private uint EncodeLui(MipsOpcode opcode, string[] operands) {
        if(operands.Length != 2) {
            throw new ArgumentException($"Expected 2 operands for {opcode}");
        }

        int rt = MipsRegisterParser.Parse(operands[0]);
        short immediate = (short)this.ParseImmediate(operands[1]);

        return MipsInstructionEncoder.EncodeIType(opcode, 0, rt, immediate);
    }

    /// <summary>
    /// 即値をパースする（10進数、16進数対応）
    /// </summary>
    private int ParseImmediate(string value) {
        value = value.Trim();

        return value.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? Convert.ToInt32(value, 16) : int.Parse(value);
    }

    /// <summary>
    /// ラベルをアドレスに解決する
    /// </summary>
    private uint ResolveLabel(string label) {
        return this._symbolTable.TryGetValue(label, out uint address) ? address : throw new ArgumentException($"Undefined label: {label}");
    }

    /// <summary>
    /// 分岐命令のオフセットを計算する（PC相対）
    /// </summary>
    private short CalculateBranchOffset(string label, uint currentAddress) {
        uint targetAddress = this.ResolveLabel(label);
        uint nextPc = currentAddress + 4; // PCは既に次の命令を指している
        int offset = (int)(targetAddress - nextPc) / 4; // ワード単位のオフセット

        return offset is < short.MinValue or > short.MaxValue
            ? throw new ArgumentException($"Branch offset out of range: {offset}")
            : (short)offset;
    }

    /// <summary>
    /// コメントと空行を削除する
    /// </summary>
    /// <param name="programLines">アセンブリ言語で書かれたプログラムの行</param>
    /// <returns>コメントと空行を削除したプログラムの行</returns>
    /// <remarks>_commentCharが埋め込み文字列中に含まれる場合は問題が生じるので注意</remarks>
    private static string[] RemoveStyle(string[] programLines) {
        return [.. programLines
            .Select(line => {
                // コメントを削除
                int commentIndex = line.IndexOf(_commentChar);
                return (commentIndex >= 0 ? line[..commentIndex] : line).Trim();
            })
            .Where(line => !string.IsNullOrEmpty(line))
        ];
    }

    /// <summary>
    /// アセンブリファイルをアセンブルする（公開メソッド）
    /// </summary>
    public void Assemble(string programPath) {
        // ファイル読み込み
        string program = File.ReadAllText(programPath);

        // 改行で分割
        string[] lines = program.Split('\n');

        // コメント・空行削除
        string[] cleanedLines = RemoveStyle(lines);

        // アセンブラ指令?

        // シンボルテーブル作成
        this.MakeSymbolTable(cleanedLines);

        // アセンブル
        _ = this.AssembleInstructions(cleanedLines);

        // TODO: machineCodeをメモリにロードする処理（IMemory経由）
        // 現時点ではバイナリを生成するまでを実装
    }
}
