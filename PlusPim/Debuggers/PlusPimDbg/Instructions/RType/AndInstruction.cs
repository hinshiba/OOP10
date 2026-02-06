using System.Diagnostics.CodeAnalysis;

namespace PlusPim.Debuggers.PlusPimDbg.Instructions.RType;

internal sealed class AndInstruction(RegisterID rd, RegisterID rs, RegisterID rt): RTypeInstruction(rd, rs, rt) {
    public override void Execute(IExecutionContext context) {
        int result = this.ReadRs(context) & this.ReadRt(context);
        this.WriteRd(context, result);
    }
}

internal sealed class AndInstructionParser: IInstructionParser {
    public string Mnemonic => "and";

    public bool TryParse(string operands, [MaybeNullWhen(false)] out IInstruction instruction) {
        instruction = null;
        if(RTypeInstruction.TryParse3RegOperands(operands, out RegisterID rd, out RegisterID rs, out RegisterID rt)) {
            instruction = new AndInstruction(rd, rs, rt);
            return true;
        }
        return false;
    }
}
