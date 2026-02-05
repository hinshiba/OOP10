using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace PlusPim.Debuggers.PlusPimDbg.Instructions;

internal abstract partial class RTypeInstruction: IInstruction {
    [GeneratedRegex(@"^\$(?<rd>\w+),\s*\$(?<rs>\w+),\s*\$(?<rt>\w+)$")]
    private static partial Regex OperandsPattern();

    protected Register Rd { get; }
    protected Register Rs { get; }
    protected Register Rt { get; }

    protected RTypeInstruction(Register rd, Register rs, Register rt) {
        this.Rd = rd;
        this.Rs = rs;
        this.Rt = rt;
    }

    public abstract void Execute(IExecutionContext context);

    internal static bool TryParseOperands(
        string operands,
        [MaybeNullWhen(false)] out Register rd,
        [MaybeNullWhen(false)] out Register rs,
        [MaybeNullWhen(false)] out Register rt) {

        rd = default;
        rs = default;
        rt = default;

        Match match = OperandsPattern().Match(operands);
        if(!match.Success) {
            return false;
        }

        if(Enum.TryParse<Register>(match.Groups["rd"].Value, true, out Register rdParsed)
            && Enum.TryParse<Register>(match.Groups["rs"].Value, true, out Register rsParsed)
            && Enum.TryParse<Register>(match.Groups["rt"].Value, true, out Register rtParsed)) {
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

    protected void WriteRegister(IExecutionContext context, Register reg, int value) {
        int index = (int)reg;
        if(index == 0) {
            return; // $zero保護
        }
        context.Registers[index] = value;
    }
}
