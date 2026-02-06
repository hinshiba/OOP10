namespace PlusPim.Debuggers.PlusPimDbg;

internal class ParsedProgram {
    private readonly Mnemonic[] _mnemonics;
    private readonly Dictionary<string, int> _symbolTable;

    public ParsedProgram(string programPath, Action<string>? log = null) {
        string[] lines = File.ReadAllLines(programPath);
        List<Mnemonic> mnemonicList = [];
        this._symbolTable = [];

        foreach(string line in lines) {
            // コメント除去 (#より後)
            string processed = line;
            int commentIndex = processed.IndexOf('#');
            if(commentIndex >= 0) {
                processed = processed[..commentIndex];
            }

            // ディレクティブ除去 (.より後)
            int dotIndex = processed.IndexOf('.');
            if(dotIndex >= 0) {
                processed = processed[..dotIndex];
            }

            // 前後の空白を除去
            processed = processed.Trim();

            // 空行はスキップ
            if(string.IsNullOrEmpty(processed)) {
                continue;
            }

            // ラベル判定: `:` で終わり、空白を含まない
            if(processed.EndsWith(':') && !processed.Contains(' ')) {
                string labelName = processed[..^1]; // 末尾の `:` を除去
                this._symbolTable[labelName] = mnemonicList.Count;
                log?.Invoke($"Label: {labelName} at index {mnemonicList.Count}");
                continue;
            }

            // ニーモニックをパース
            if(Mnemonic.TryParse(processed, null, out Mnemonic? mnemonic)) {
                mnemonicList.Add(mnemonic);
                log?.Invoke($"Parsed: {processed}");
            } else {
                log?.Invoke($"Parse failed: {processed}");
            }
        }

        this._mnemonics = mnemonicList.ToArray();
    }

    public Mnemonic GetMnemonic(int index) {
        return this._mnemonics[index];
    }

    public int MnemonicCount => this._mnemonics.Length;
    public int? GetLabelAddress(string label) {
        return this._symbolTable.TryGetValue(label, out int addr) ? addr : null;
    }
}
