using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace PlusPim.Debuggers.PlusPimDbg.Instructions;

/// <summary>
/// MIPSにおいてR形式の命令を表す抽象基底クラス
/// </summary>
internal abstract partial class RTypeInstruction: IInstruction {
    [GeneratedRegex(@"^\$(?<rd>\w+),\s*\$(?<rs>\w+),\s*\$(?<rt>\w+)$")]
    private static partial Regex OperandsPattern();

    protected RegisterID Rd { get; }
    protected RegisterID Rs { get; }
    protected RegisterID Rt { get; }

    protected RTypeInstruction(RegisterID rd, RegisterID rs, RegisterID rt) {
        this.Rd = rd;
        this.Rs = rs;
        this.Rt = rt;
    }

    public abstract void Execute(IExecutionContext context);

    internal static bool TryParseOperands(
        string operands,
        [MaybeNullWhen(false)] out RegisterID rd,
        [MaybeNullWhen(false)] out RegisterID rs,
        [MaybeNullWhen(false)] out RegisterID rt) {

        rd = default;
        rs = default;
        rt = default;

        Match match = OperandsPattern().Match(operands);
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
