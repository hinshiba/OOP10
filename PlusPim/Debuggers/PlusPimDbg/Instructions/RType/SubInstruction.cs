using System.Diagnostics.CodeAnalysis;

namespace PlusPim.Debuggers.PlusPimDbg.Instructions.RType;

internal sealed class SubInstruction : RTypeInstruction {
    public SubInstruction(Register rd, Register rs, Register rt) : base(rd, rs, rt) { }

    public override void Execute(IExecutionContext context) {
        int result = ReadRs(context) - ReadRt(context);
        WriteRegister(context, Rd, result);
    }
}

internal sealed class SubInstructionParser : IInstructionParser {
    public string Mnemonic => "sub";

    public bool TryParse(string operands, [MaybeNullWhen(false)] out IInstruction instruction) {
        instruction = null;
        if (RTypeInstruction.TryParseOperands(operands, out Register rd, out Register rs, out Register rt)) {
            instruction = new SubInstruction(rd, rs, rt);
            return true;
        }
        return false;
    }
}
