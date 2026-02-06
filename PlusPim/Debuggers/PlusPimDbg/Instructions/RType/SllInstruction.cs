using System.Diagnostics.CodeAnalysis;

namespace PlusPim.Debuggers.PlusPimDbg.Instructions.RType;

internal sealed class SllInstruction(RegisterID rd, RegisterID rt, Immediate shamt): RTypeInstruction(rd, rt, shamt) {
    public override void Execute(IExecutionContext context) {
        int result = this.ReadRt(context) << this.Shamt;
        this.WriteRd(context, result);
    }
}

internal sealed class SllInstructionParser: IInstructionParser {
    public string Mnemonic => "sll";

    public bool TryParse(string operands, [MaybeNullWhen(false)] out IInstruction instruction) {
        instruction = null;
        if(RTypeInstruction.TryParse2RegShamtOperands(operands, out RegisterID rd, out RegisterID rt, out Immediate? shamt)) {
            instruction = new SllInstruction(rd, rt, shamt);
            return true;
        }
        return false;
    }
}
