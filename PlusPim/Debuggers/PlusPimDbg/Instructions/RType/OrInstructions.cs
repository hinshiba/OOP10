using System.Diagnostics.CodeAnalysis;

namespace PlusPim.Debuggers.PlusPimDbg.Instructions.RType;

internal sealed class OrInstruction: RTypeInstruction {
    public OrInstruction(Register rd, Register rs, Register rt) : base(rd, rs, rt) { }

    public override void Execute(IExecutionContext context) {
        int result = this.ReadRs(context) | this.ReadRt(context);
        this.WriteRegister(context, this.Rd, result);
    }
}

internal sealed class AddInstructionParser: IInstructionParser {
    public string Mnemonic => "or";

    public bool TryParse(string operands, [MaybeNullWhen(false)] out IInstruction instruction) {
        instruction = null;
        if(RTypeInstruction.TryParseOperands(operands, out Register rd, out Register rs, out Register rt)) {
            instruction = new OrInstruction(rd, rs, rt);
            return true;
        }
        return false;
    }
}