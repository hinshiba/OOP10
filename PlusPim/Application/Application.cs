namespace PlusPim.Application;

internal class Application: IApplication {
    private readonly IDebugger _debugger;

    internal Application(IDebugger debugger) {
        this._debugger = debugger;
    }

    public bool Load(string programPath) {
        return this._debugger.Load(programPath);
    }

    public (int[] Registers, int PC, int HI, int LO) GetRegisters() {
        return (this._debugger.GetRegisters(), this._debugger.GetPC(), this._debugger.GetHI(), this._debugger.GetLO());
    }
}
