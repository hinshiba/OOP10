namespace PlusPim.Runtime.Services;

internal interface IAssembler {
    /// <summary>
    /// パスのプログラムをアセンブルする．
    /// </summary>
    /// <param name="programPath">プログラムのパス</param>
    void Assemble(string programPath);
}
