namespace PlusPim.Debuggers.PlusPimDbg;

internal interface IExecutionContext {
    int[] Registers { get; }
    int PC { get; set; }
    int HI { get; set; }
    int LO { get; set; }

    Dictionary<int, byte> Memory { get; }
}

/// <summary>
/// 実行に必要なレジスタ，特殊レジスタ，メモリ情報を提供する
/// </summary>
internal sealed class ExecuteContext: IExecutionContext {
    public ExecuteContext(int[] registers) {
        this.Registers = registers;
    }

    public int[] Registers { get; }
    public int PC { get; set; }
    public int HI { get; set; }
    public int LO { get; set; }

    public Dictionary<int, byte> Memory { get; } = [];
}
