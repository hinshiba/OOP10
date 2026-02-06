using System.Diagnostics.CodeAnalysis;

namespace PlusPim.Debuggers.PlusPimDbg.Instructions.RType;

internal sealed class SltInstruction: RTypeInstruction {
    public SltInstruction(RegisterID rd, RegisterID rs, RegisterID rt) : base(rd, rs, rt) { }

    public override void Execute(IExecutionContext context) {
        int result = this.ReadRs(context) < this.ReadRt(context) ? 1 : 0;
        this.WriteRd(context, result);
    }
}

internal sealed class SltInstructionParser: IInstructionParser {
    public string Mnemonic => "slt";

    public bool TryParse(string operands, [MaybeNullWhen(false)] out IInstruction instruction) {
        instruction = null;
        if(RTypeInstruction.TryParse3RegOperands(operands, out RegisterID rd, out RegisterID rs, out RegisterID rt)) {
            instruction = new SltInstruction(rd, rs, rt);
            return true;
        }
        return false;
    }
}
