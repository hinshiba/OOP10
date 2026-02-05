using System.Diagnostics.CodeAnalysis;

namespace PlusPim.Debuggers.PlusPimDbg.Instructions.RType;

internal sealed class AdduInstruction: RTypeInstruction {
    public AdduInstruction(Register rd, Register rs, Register rt) : base(rd, rs, rt) { }

    public override void Execute(IExecutionContext context) {
        int result = this.ReadRs(context) + this.ReadRt(context);
        this.WriteRegister(context, this.Rd, result);
    }
}

internal sealed class AdduInstructionParser: IInstructionParser {
    public string Mnemonic => "addu";

    public bool TryParse(string operands, [MaybeNullWhen(false)] out IInstruction instruction) {
        instruction = null;
        if(RTypeInstruction.TryParseOperands(operands, out Register rd, out Register rs, out Register rt)) {
            instruction = new AdduInstruction(rd, rs, rt);
            return true;
        }
        return false;
    }
}
