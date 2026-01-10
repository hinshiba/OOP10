namespace PlusPim.Runtime.Services;

internal interface IMemory {
    void Reset();
    void LoadProgram(byte[] program);
}
