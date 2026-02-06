using System.Diagnostics.CodeAnalysis;

namespace PlusPim.Debuggers.PlusPimDbg.Instructions.RType;

internal sealed class OrInstruction: RTypeInstruction {
    public OrInstruction(RegisterID rd, RegisterID rs, RegisterID rt) : base(rd, rs, rt) { }

    public override void Execute(IExecutionContext context) {
        int result = this.ReadRs(context) | this.ReadRt(context);
        this.WriteRd(context, result);
    }
}

internal sealed class OrInstructionParser: IInstructionParser {
    public string Mnemonic => "or";

    public bool TryParse(string operands, [MaybeNullWhen(false)] out IInstruction instruction) {
        instruction = null;
        if(RTypeInstruction.TryParse3RegOperands(operands, out RegisterID rd, out RegisterID rs, out RegisterID rt)) {
            instruction = new OrInstruction(rd, rs, rt);
            return true;
        }
        return false;
    }
}
