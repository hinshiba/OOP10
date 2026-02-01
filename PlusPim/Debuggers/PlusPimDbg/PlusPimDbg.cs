using PlusPim.Application;

namespace PlusPim.Debuggers.PlusPimDbg;

internal class PlusPimDbg: IDebugger {
    private readonly int[] _registers = new int[32];
    private int _pc;
    private int _hi;
    private int _lo;

    public bool Load(string programPath) {
        // レジスタを初期化（全て0）
        Array.Clear(this._registers, 0, 32);
        this._pc = 0x00400000;
        this._hi = 0;
        this._lo = 0;
        return true;
    }

    public int GetPC() {
        return this._pc;
    }

    public int[] GetRegisters() {
        return this._registers;
    }

    public int GetHI() {
        return this._hi;
    }

    public int GetLO() {
        return this._lo;
    }
}
