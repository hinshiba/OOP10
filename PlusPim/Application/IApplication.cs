namespace PlusPim.Application;

internal interface IApplication {
    bool Load(string programPath);
    (int[] Registers, int PC, int HI, int LO) GetRegisters();
}
