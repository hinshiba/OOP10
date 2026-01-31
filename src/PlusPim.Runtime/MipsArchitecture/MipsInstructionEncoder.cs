namespace PlusPim.Runtime.MipsArchitecture;

/// <summary>
/// MIPS命令をバイナリ形式にエンコードするユーティリティクラス
/// </summary>
internal static class MipsInstructionEncoder {
    /// <summary>
    /// R-type命令をエンコードする
    /// Format: opcode(6) | rs(5) | rt(5) | rd(5) | shamt(5) | funct(6)
    /// </summary>
    /// <param name="funct">機能コード</param>
    /// <param name="rs">ソースレジスタ1</param>
    /// <param name="rt">ソースレジスタ2/ターゲットレジスタ</param>
    /// <param name="rd">デスティネーションレジスタ</param>
    /// <param name="shamt">シフト量</param>
    /// <returns>32ビットのマシンコード</returns>
    public static uint EncodeRType(MipsRtypeFunction funct, int rs, int rt, int rd, int shamt = 0) {
        uint opcode = (uint)MipsOpcode.RType;
        uint instruction = 0;

        instruction |= (opcode & 0x3F) << 26;        // opcode: bits 31-26
        instruction |= ((uint)rs & 0x1F) << 21;      // rs: bits 25-21
        instruction |= ((uint)rt & 0x1F) << 16;      // rt: bits 20-16
        instruction |= ((uint)rd & 0x1F) << 11;      // rd: bits 15-11
        instruction |= ((uint)shamt & 0x1F) << 6;    // shamt: bits 10-6
        instruction |= (uint)funct & 0x3F;         // funct: bits 5-0

        return instruction;
    }

    /// <summary>
    /// I-type命令をエンコードする
    /// Format: opcode(6) | rs(5) | rt(5) | immediate(16)
    /// </summary>
    /// <param name="opcode">オペコード</param>
    /// <param name="rs">ソースレジスタ</param>
    /// <param name="rt">ターゲットレジスタ</param>
    /// <param name="immediate">即値（16ビット符号付き）</param>
    /// <returns>32ビットのマシンコード</returns>
    public static uint EncodeIType(MipsOpcode opcode, int rs, int rt, short immediate) {
        uint instruction = 0;

        instruction |= ((uint)opcode & 0x3F) << 26;  // opcode: bits 31-26
        instruction |= ((uint)rs & 0x1F) << 21;      // rs: bits 25-21
        instruction |= ((uint)rt & 0x1F) << 16;      // rt: bits 20-16
        instruction |= (uint)immediate & 0xFFFF;     // immediate: bits 15-0

        return instruction;
    }

    /// <summary>
    /// J-type命令をエンコードする
    /// Format: opcode(6) | address(26)
    /// </summary>
    /// <param name="opcode">オペコード</param>
    /// <param name="address">ジャンプ先アドレス（ワードアドレス）</param>
    /// <returns>32ビットのマシンコード</returns>
    public static uint EncodeJType(MipsOpcode opcode, uint address) {
        uint instruction = 0;

        instruction |= ((uint)opcode & 0x3F) << 26;  // opcode: bits 31-26
        instruction |= address & 0x03FFFFFF;         // address: bits 25-0

        return instruction;
    }

    /// <summary>
    /// 32ビット命令をビッグエンディアンのバイト配列に変換する
    /// </summary>
    /// <param name="instruction">32ビットマシンコード</param>
    /// <returns>4バイトの配列（ビッグエンディアン）</returns>
    public static byte[] ToBytes(uint instruction) {
        return [
            (byte)((instruction >> 24) & 0xFF),
            (byte)((instruction >> 16) & 0xFF),
            (byte)((instruction >> 8) & 0xFF),
            (byte)(instruction & 0xFF)
        ];
    }
}
