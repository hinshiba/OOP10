using PlusPim.Runtime.Services;
using System.Text.RegularExpressions;

namespace PlusPim.Runtime.MipsArchitecture;

internal partial class MipsAsembler: IAssembler {
    private const int _initialAddress = 0x00400000;
    private const char _commentChar = ';';


    [GeneratedRegex(@"^[a-zA-Z_][a-zA-Z0-9_]+:$", RegexOptions.Compiled)]
    private static partial Regex _labelRegex();

    private readonly Dictionary<string, int> _symbolTable = [];



    /// <summary>
    /// 
    /// </summary>
    private void MakeSymbolTable(string[] programStrs) {

    }

    /// <summary>
    /// コメントと空行を削除する
    /// </summary>
    /// <param name="programLines"> アセンブリ言語でかかれたプログラムの行 </param>
    /// <returns> コメントと空行を削除したプログラムの行 </returns>
    /// <remarks>文字列中にコメントcharがあると問題が発生します</remarks>
    private static string[] RemoveStyle(string[] programLines) {
        // readonly spanという手もあるらしい
        return [.. programLines
            .Select(line => { // コメントを削除
                int commentIndex = line.IndexOf(_commentChar);
                return (commentIndex >= 0 ? line[..commentIndex] : line).Trim();
            })
            .Where(line => !string.IsNullOrEmpty(line))    // 空行を削除
            ];
    }

    public void Assemble(string programPath) {
        // ファイル読み込み
        string program = File.ReadAllText(programPath);

        // 改行で分割
        string[] lines = program.Split('\n');
        _ = RemoveStyle(lines);

        throw new NotImplementedException();
    }
}
