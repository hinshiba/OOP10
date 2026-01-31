namespace PlusPim.Application;

internal interface IApplication {
    void initialize();
    void launch();
    void disconnect();
}
