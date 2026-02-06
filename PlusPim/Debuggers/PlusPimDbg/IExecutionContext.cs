namespace PlusPim.Debuggers.PlusPimDbg;

internal interface IExecutionContext {
    int[] Registers { get; }
    int PC { get; }
    int ExecutionIndex { get; set; }
    int HI { get; set; }
    int LO { get; set; }
    byte ReadMemoryByte(int address);
    void WriteMemoryByte(int address, byte value);

}

/// <summary>
/// 実行に必要なレジスタ，特殊レジスタ，メモリ情報を提供する
/// </summary>
internal sealed class ExecuteContext: IExecutionContext {
    public int[] Registers { get; }
    public int PC => this.ExecutionIndex + 0x00400000;

    /// PCの実装の代わり
    public int ExecutionIndex { get; set; }

    public int HI { get; set; }
    public int LO { get; set; }

    /// <summary>
    /// メモリ空間の表現
    /// アクセス前は未初期化(0扱い)
    /// </summary>
    private readonly Dictionary<int, byte> Memory;
    public ExecuteContext() {
        this.Registers = new int[32];
        // 未初期化のうほうが現実的
        //Array.Clear(this.Registers, 0, 32);
        // HI LO も同様

        // PCの代わりのExecutionIndexは初期化
        this.ExecutionIndex = 0;

        // メモリは暗黙的には0扱い
        this.Memory = [];
    }

    public byte ReadMemoryByte(int address) {
        return this.Memory.TryGetValue(address, out byte value) ? value : (byte)0;
    }

    public void WriteMemoryByte(int address, byte value) {
        this.Memory[address] = value;
    }

}
