namespace PlusPim.Runtime.MipsArchitecture;

/// <summary>
/// MIPSレジスタ名をレジスタ番号に変換するパーサー
/// </summary>
internal static class MipsRegisterParser {
    private static readonly Dictionary<string, int> _registerMap = new() {
        // Numeric format
        ["$0"] = 0,
        ["$1"] = 1,
        ["$2"] = 2,
        ["$3"] = 3,
        ["$4"] = 4,
        ["$5"] = 5,
        ["$6"] = 6,
        ["$7"] = 7,
        ["$8"] = 8,
        ["$9"] = 9,
        ["$10"] = 10,
        ["$11"] = 11,
        ["$12"] = 12,
        ["$13"] = 13,
        ["$14"] = 14,
        ["$15"] = 15,
        ["$16"] = 16,
        ["$17"] = 17,
        ["$18"] = 18,
        ["$19"] = 19,
        ["$20"] = 20,
        ["$21"] = 21,
        ["$22"] = 22,
        ["$23"] = 23,
        ["$24"] = 24,
        ["$25"] = 25,
        ["$26"] = 26,
        ["$27"] = 27,
        ["$28"] = 28,
        ["$29"] = 29,
        ["$30"] = 30,
        ["$31"] = 31,

        // Named registers
        ["$zero"] = 0,
        ["$at"] = 1,
        ["$v0"] = 2,
        ["$v1"] = 3,
        ["$a0"] = 4,
        ["$a1"] = 5,
        ["$a2"] = 6,
        ["$a3"] = 7,
        ["$t0"] = 8,
        ["$t1"] = 9,
        ["$t2"] = 10,
        ["$t3"] = 11,
        ["$t4"] = 12,
        ["$t5"] = 13,
        ["$t6"] = 14,
        ["$t7"] = 15,
        ["$s0"] = 16,
        ["$s1"] = 17,
        ["$s2"] = 18,
        ["$s3"] = 19,
        ["$s4"] = 20,
        ["$s5"] = 21,
        ["$s6"] = 22,
        ["$s7"] = 23,
        ["$t8"] = 24,
        ["$t9"] = 25,
        ["$k0"] = 26,
        ["$k1"] = 27,
        ["$gp"] = 28,
        ["$sp"] = 29,
        ["$fp"] = 30,
        ["$s8"] = 30,
        ["$ra"] = 31,
    };

    /// <summary>
    /// レジスタ名をレジスタ番号に変換する
    /// </summary>
    /// <param name="registerName">レジスタ名 (例: "$t0", "$5")</param>
    /// <param name="registerNumber">レジスタ番号 (0-31)</param>
    /// <returns>変換に成功した場合はtrue</returns>
    public static bool TryParse(string registerName, out int registerNumber) {
        return _registerMap.TryGetValue(registerName.ToLower(), out registerNumber);
    }

    /// <summary>
    /// レジスタ名をレジスタ番号に変換する（例外をスローする版）
    /// </summary>
    /// <param name="registerName">レジスタ名</param>
    /// <returns>レジスタ番号 (0-31)</returns>
    /// <exception cref="ArgumentException">無効なレジスタ名</exception>
    public static int Parse(string registerName) {
        if(TryParse(registerName, out int registerNumber)) {
            return registerNumber;
        }
        throw new ArgumentException($"Invalid register name: {registerName}", nameof(registerName));
    }
}
