namespace PlusPim.Application;

internal class Application: IApplication {
    private readonly IDebugger _debugger;

    internal Application(IDebugger debugger) {
        this._debugger = debugger;
    }

    public bool Load(string programPath) {
        return this._debugger.Load(programPath);
    }
}
