using PlusPim.Application;

namespace PlusPim.Debuggers.PlusPimDbg;

internal class PlusPimDbg: IDebugger {
    private ExecuteContext? _context;
    private ParsedProgram? _program;
    private bool _isTerminated;
    private Action<string>? _log;

    public void SetLogger(Action<string> log) {
        this._log = log;
    }

    public bool Load(string programPath) {
        // プログラムの解析
        this._program = new ParsedProgram(programPath, this._log);
        // 実行コンテキストの初期化
        this._context = new ExecuteContext(this._log);
        this._context.Registers[(int)RegisterID.T1] = 0xcafe; // テスト用初期値
        this._context.ExecutionIndex = this._program.GetLabelAddress("main") ?? 0;
        this._isTerminated = false;
        return true;
    }

    public int GetPC() {

        return this._context?.PC ?? -1;
    }

    public int[] GetRegisters() {
        return this._context?.Registers ?? [];
    }

    public int GetHI() {
        return this._context?.HI ?? -1;
    }

    public int GetLO() {
        return this._context?.LO ?? -1;
    }

    public void Step() {
        // プログラムの終了か未初期化チェック
        if(this._isTerminated || this._program == null || this._context == null) {
            return;
        }

        if(this._program.MnemonicCount <= this._context.ExecutionIndex) {
            this._isTerminated = true;
            return;
        }

        this._program.GetMnemonic(this._context.ExecutionIndex).Execute(this._context);
        this._context.ExecutionIndex++;

        if(this._program.MnemonicCount <= this._context.ExecutionIndex) {
            this._isTerminated = true;
        }
    }

    public int GetCurrentLine() {
        return this._context == null ? -1 : this._context.ExecutionIndex;
    }

    public bool IsTerminated() {
        return this._isTerminated;
    }
}
