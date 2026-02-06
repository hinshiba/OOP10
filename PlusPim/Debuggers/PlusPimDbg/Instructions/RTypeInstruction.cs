using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace PlusPim.Debuggers.PlusPimDbg.Instructions;

/// <summary>
/// MIPSにおいてR形式の命令を表す抽象基底クラス
/// </summary>
internal abstract partial class RTypeInstruction: IInstruction {
    [GeneratedRegex(@"^\$(?<rd>\w+),\s*\$(?<rs>\w+),\s*\$(?<rt>\w+)$")]
    private static partial Regex OperandsPattern();

    // オペランドのレジスタID
    protected RegisterID Rd { get; }
    protected RegisterID Rs { get; }
    protected RegisterID Rt { get; }

    // シフト量の即値
    protected int Shamt { get; }

    protected RTypeInstruction(RegisterID rd, RegisterID rs, RegisterID rt) {
        this.Rd = rd;
        this.Rs = rs;
        this.Rt = rt;
    }

    public abstract void Execute(IExecutionContext context);

    /// <summary>
    /// オペランドを3レジスタを指定している文字列から解析してRegisterIDに変換する
    /// </summary>
    /// <param name="operands">対象の文字列</param>
    /// <param name="rd"><see langword="true"/>ならばRegsiterIDが代入される．<see langword="false"/>のときの値は未定義．</param>
    /// <param name="rs">rdと同様</param>
    /// <param name="rt">rdと同様</param>
    /// <returns><see langword="true"/>ならば解析成功</returns>
    internal static bool TryParse3RegOperands(
        string operands,
        [MaybeNullWhen(false)] out RegisterID rd,
        [MaybeNullWhen(false)] out RegisterID rs,
        [MaybeNullWhen(false)] out RegisterID rt) {

        rd = default;
        rs = default;
        rt = default;

        Match match = Operands3RegPattern().Match(operands);
        if(!match.Success) {
            return false;
        }

        if(Enum.TryParse<RegisterID>(match.Groups["rd"].Value, true, out RegisterID rdParsed)
            && Enum.TryParse<RegisterID>(match.Groups["rs"].Value, true, out RegisterID rsParsed)
            && Enum.TryParse<RegisterID>(match.Groups["rt"].Value, true, out RegisterID rtParsed)) {
            rd = rdParsed;
            rs = rsParsed;
            rt = rtParsed;
            return true;
        }

        return false;
    }

    protected int ReadRs(IExecutionContext context) {
        return context.Registers[(int)this.Rs];
    }

    protected int ReadRt(IExecutionContext context) {
        return context.Registers[(int)this.Rt];
    }

    protected void WriteRegister(IExecutionContext context, RegisterID reg, int value) {
        int index = (int)reg;
        if(index == 0) {
            return; // $zero保護
        }
        context.Registers[index] = value;
    }
}
