using PlusPim.EditorController.DebugAdapter;

namespace PlusPim;

internal class Program {
    private static void Main(string[] args) {
        Application
        // 各種クラスを呼び出す
        _ = new
        DebugAdapter(Console.OpenStandardInput(), Console.OpenStandardOutput());

    }
}
