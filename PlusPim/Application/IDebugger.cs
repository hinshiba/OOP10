namespace PlusPim.Application;

internal interface IDebugger {
    void SetLogger(Action<string> log);
    bool Load(string programPath);
    int GetPC();
    int[] GetRegisters();  // 32要素の配列
    int GetHI();
    int GetLO();
    void Step();
    int GetCurrentLine();
    string GetProgramPath();
    bool IsTerminated();
}
