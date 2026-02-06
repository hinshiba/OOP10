namespace PlusPim.Debuggers.PlusPimDbg;

internal interface IExecutionContext {
    int[] Registers { get; }
    int PC { get; set; }
    int HI { get; set; }
    int LO { get; set; }
}

internal sealed class RegisterOnlyContext: IExecutionContext {
    public RegisterOnlyContext(int[] registers) {
        this.Registers = registers;
    }

    public int[] Registers { get; }
    public int PC { get; set; }
    public int HI { get; set; }
    public int LO { get; set; }
}
