namespace PlusPim.Application;

internal interface IApplication {
    void SetLogger(Action<string> log);
    bool Load(string programPath);
    (int[] Registers, int PC, int HI, int LO) GetRegisters();
    void Step();
    int GetCurrentLine();
    string GetProgramPath();
    bool IsTerminated();
}
