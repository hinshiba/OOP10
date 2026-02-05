namespace PlusPim.Debuggers.PlusPimDbg;

internal interface IExecutionContext {
    int[] Registers { get; }
    int PC { get; set; }
    int HI { get; set; }
    int LO { get; set; }
}

internal sealed class RegisterOnlyContext : IExecutionContext {
    private readonly int[] _registers;

    public RegisterOnlyContext(int[] registers) {
        _registers = registers;
    }

    public int[] Registers => _registers;
    public int PC { get; set; }
    public int HI { get; set; }
    public int LO { get; set; }
}
