using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace PlusPim.Debuggers.PlusPimDbg;

internal partial class Mnemonic: IParsable<Mnemonic> {
    [GeneratedRegex(@"^(?<op>\w+)\s+\$(?<rd>\w{2}),\s*\$(?<rs>\w{2}),\s*\$(?<rt>\w{2})$")]
    private static partial Regex _reg3Pattern();

    private static readonly Dictionary<string, OpRegs3> _opRegs3Names = new() {
        { "add", OpRegs3.Add },
        { "addu", OpRegs3.Addu },
        { "sub", OpRegs3.Sub },
    };

    private OperationType _operationType;
    private OpRegs3 _opRegs3;
    private Register _rd;
    private Register _rs;
    private Register _rt;


    private OperationType OperationType { get; set; }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Mnemonic result) {
        if(s is null) {
            result = null;
            return false;
        }
        result = new Mnemonic();
        if(result.TryParseReg3(s)) {
            return true;
        }
        // todo
        return false;
    }

    static Mnemonic IParsable<Mnemonic>.Parse(string? s, IFormatProvider? provider) {
        return Mnemonic.TryParse(s, provider, out Mnemonic? result) ? result : throw new FormatException();
    }



    private bool TryParseReg3([NotNullWhen(true)] string s) {
        Match match = _reg3Pattern().Match(s);
        if(_opRegs3Names.TryGetValue(match.Groups["op"].Value, out OpRegs3 opRegs3)) {
            this._operationType = OperationType.Regs3;
            this._opRegs3 = opRegs3;

            // ignore caseでパース
            if(Enum.TryParse<Register>(match.Groups["rd"].Value, true, out Register rd)
                && Enum.TryParse<Register>(match.Groups["rs"].Value, true, out Register rs)
                && Enum.TryParse<Register>(match.Groups["rt"].Value, true, out Register rt)) {
                this._rd = rd;
                this._rs = rs;
                this._rt = rt;
                return true;
            } else {
                return false;
            }
        } else {
            return false;
        }
    }

    public void Execute(int[] registers) {
        switch(this._operationType) {
            case OperationType.Regs3:
                this.ExecuteRegs3(registers);
                break;
        }
    }

    private void ExecuteRegs3(int[] registers) {
        int rdIndex = (int)this._rd;
        if(rdIndex == 0) {
            return; // $zero保護
        }

        int rsValue = registers[(int)this._rs];
        int rtValue = registers[(int)this._rt];

        registers[rdIndex] = this._opRegs3 switch {
            OpRegs3.Add => rsValue + rtValue,
            OpRegs3.Addu => rsValue + rtValue,
            OpRegs3.Sub => rsValue - rtValue,
            _ => registers[rdIndex]
        };
    }
}








internal enum OperationType {
    Jump,
    Memory,
    Regs3,
    Immediate,
}


internal enum OpRegs3 {
    Add,
    Addu,
    Sub
}

internal enum OpImm {
    Addi,
    Addiu,

}


internal enum Register {
    Zero,
    At,
    V0,
    V1,
    A0,
    A1,
    A2,
    A3,
    T0,
    T1,
    T2,
    T3,
    T4,
    T5,
    T6,
    T7,
    S0,
    S1,
    S2,
    S3,
    S4,
    S5,
    S6,
    S7,
    T8,
    T9,
    K0,
    K1,
    Gp,
    Sp,
    S8,
    Ra
}
