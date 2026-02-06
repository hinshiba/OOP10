using System.Diagnostics.CodeAnalysis;

namespace PlusPim.Debuggers.PlusPimDbg.Instructions.RType;

internal sealed class AndInstruction: RTypeInstruction {
    public AndInstruction(Register rd, Register rs, Register rt) : base(rd, rs, rt) { }

    public override void Execute(IExecutionContext context) {
        int result = this.ReadRs(context) & this.ReadRt(context);
        this.WriteRegister(context, this.Rd, result);
    }
}

internal sealed class AndInstructionParser: IInstructionParser {
    public string Mnemonic => "and";

    public bool TryParse(string operands, [MaybeNullWhen(false)] out IInstruction instruction) {
        instruction = null;
        if(RTypeInstruction.TryParseOperands(operands, out Register rd, out Register rs, out Register rt)) {
            instruction = new AndInstruction(rd, rs, rt);
            return true;
        }
        return false;
    }
}
