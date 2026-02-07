namespace PlusPim.Application;

internal interface IApplication {
    void SetLogger(Action<string> log);
    bool Load(string programPath);
    (int[] Registers, int PC, int HI, int LO) GetRegisters();
    void Step();

    /// <summary>
    /// 1ステップ分、実行を巻き戻す
    /// </summary>
    /// <returns>巻き戻しに成功した場合は<see langword="true"/></returns>
    bool StepBack();
    int GetCurrentLine();
    string GetProgramPath();
    bool IsTerminated();
}
