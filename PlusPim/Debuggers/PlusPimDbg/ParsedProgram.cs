namespace PlusPim.Debuggers.PlusPimDbg;

internal class ParsedProgram {
    private readonly Mnemonic[] _mnemonics;
    private readonly Dictionary<string, int> _symbolTable;

    public ParsedProgram(string programPath) {
        var lines = File.ReadAllLines(programPath);
        var mnemonicList = new List<Mnemonic>();
        _symbolTable = new Dictionary<string, int>();

        foreach (var line in lines) {
            // コメント除去 (#より後)
            var processed = line;
            var commentIndex = processed.IndexOf('#');
            if (commentIndex >= 0) {
                processed = processed[..commentIndex];
            }

            // ディレクティブ除去 (.より後)
            var dotIndex = processed.IndexOf('.');
            if (dotIndex >= 0) {
                processed = processed[..dotIndex];
            }

            // 前後の空白を除去
            processed = processed.Trim();

            // 空行はスキップ
            if (string.IsNullOrEmpty(processed)) {
                continue;
            }

            // ラベル判定: `:` で終わり、空白を含まない
            if (processed.EndsWith(':') && !processed.Contains(' ')) {
                var labelName = processed[..^1]; // 末尾の `:` を除去
                _symbolTable[labelName] = mnemonicList.Count;
                continue;
            }

            // ニーモニックをパース
            if (Mnemonic.TryParse(processed, null, out var mnemonic)) {
                mnemonicList.Add(mnemonic);
            }
            // パース失敗時は無視してスキップ
        }

        _mnemonics = mnemonicList.ToArray();
    }
}
