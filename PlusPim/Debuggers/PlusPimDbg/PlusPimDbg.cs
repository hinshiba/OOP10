using PlusPim.Application;

namespace PlusPim.Debuggers.PlusPimDbg;

internal class PlusPimDbg: IDebugger {
    private readonly int[] _registers = new int[32];
    private int _pc;
    private int _hi;
    private int _lo;
    private ParsedProgram? _program;
    private int _executionIndex;
    private bool _isTerminated;

    public bool Load(string programPath) {
        Array.Clear(this._registers, 0, 32);
        this._registers[(int)Register.T1] = 0xcafe; // テスト用初期値
        this._pc = 0x00400000;
        this._hi = 0;
        this._lo = 0;

        this._program = new ParsedProgram(programPath);
        this._executionIndex = this._program.GetLabelAddress("main") ?? 0;
        this._isTerminated = false;
        // for dbg
        this._hi = this._program.MnemonicCount;
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

    public void Step() {
        if(this._program == null || this._isTerminated) {
            return;
        }

        if(this._executionIndex >= this._program.MnemonicCount) {
            this._isTerminated = true;
            return;
        }

        this._program.GetMnemonic(this._executionIndex).Execute(this._registers);
        this._executionIndex++;

        if(this._executionIndex >= this._program.MnemonicCount) {
            this._isTerminated = true;
        }
    }

    public int GetCurrentLine() {
        return this._executionIndex;
    }

    public bool IsTerminated() {
        return this._isTerminated;
    }
}
