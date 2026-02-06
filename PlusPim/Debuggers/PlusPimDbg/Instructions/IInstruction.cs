namespace PlusPim.Debuggers.PlusPimDbg.Instructions;

internal interface IInstruction {
    void Execute(IExecutionContext context);
}
