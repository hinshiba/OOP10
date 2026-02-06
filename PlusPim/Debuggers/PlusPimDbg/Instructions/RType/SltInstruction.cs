using System.Diagnostics.CodeAnalysis;

namespace PlusPim.Debuggers.PlusPimDbg.Instructions.RType;

internal sealed class SltInstruction: RTypeInstruction {
    public SltInstruction(Register rd, Register rs, Register rt) : base(rd, rs, rt) { }

    public override void Execute(IExecutionContext context) {
        int result = this.ReadRs(context) < this.ReadRt(context) ? 1 : 0;
        this.WriteRegister(context, this.Rd, result);
    }
}

internal sealed class SltInstructionParser: IInstructionParser {
    public string Mnemonic => "slt";

    public bool TryParse(string operands, [MaybeNullWhen(false)] out IInstruction instruction) {
        instruction = null;
        if(RTypeInstruction.TryParseOperands(operands, out Register rd, out Register rs, out Register rt)) {
            instruction = new SltInstruction(rd, rs, rt);
            return true;
        }
        return false;
    }
}
