namespace PlusPim.Runtime;

internal interface IRuntimeService {
    void LoadProgram(string path);
    void Reset();
    void ExecuteSteps(uint stepNum);

}

