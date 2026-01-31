using System.ComponentModel;

namespace PlusPim.Runtime.MipsArchitecture;

/// <summary>
/// MIPSの命令オペコードを表す列挙型
/// </summary>
internal enum MipsOpcode: uint {
    // R-Type (opcode = 0, funct field distinguishes)
    [Description("special")]
    RType = 0b000000,

    // I-Type - Arithmetic
    [Description("addi")]
    Addi = 0b001000,
    [Description("addiu")]
    Addiu = 0b001001,
    [Description("slti")]
    Slti = 0b001010,
    [Description("sltiu")]
    Sltiu = 0b001011,

    // I-Type - Logical
    [Description("andi")]
    Andi = 0b001100,
    [Description("ori")]
    Ori = 0b001101,
    [Description("xori")]
    Xori = 0b001110,
    [Description("lui")]
    Lui = 0b001111,

    // I-Type - Branch
    [Description("beq")]
    Beq = 0b000100,
    [Description("bne")]
    Bne = 0b000101,
    [Description("blez")]
    Blez = 0b000110,
    [Description("bgtz")]
    Bgtz = 0b000111,
    [Description("bltz_bgez")]
    BltzBgez = 0b000001, // Uses rt field to distinguish

    // I-Type - Memory Operations
    [Description("lb")]
    Lb = 0b100000,
    [Description("lh")]
    Lh = 0b100001,
    [Description("lw")]
    Lw = 0b100011,
    [Description("lbu")]
    Lbu = 0b100100,
    [Description("lhu")]
    Lhu = 0b100101,
    [Description("sb")]
    Sb = 0b101000,
    [Description("sh")]
    Sh = 0b101001,
    [Description("sw")]
    Sw = 0b101011,

    // J-Type
    [Description("j")]
    J = 0b000010,
    [Description("jal")]
    Jal = 0b000011,
}

/// <summary>
/// MIPSのRタイプ命令の機能フィールドを表す列挙型
/// </summary>
internal enum MipsRtypeFunction: uint {
    // Shift operations
    [Description("sll")]
    Sll = 0b000000,
    [Description("srl")]
    Srl = 0b000010,
    [Description("sra")]
    Sra = 0b000011,
    [Description("sllv")]
    Sllv = 0b000100,
    [Description("srlv")]
    Srlv = 0b000110,
    [Description("srav")]
    Srav = 0b000111,

    // Jump Register
    [Description("jr")]
    Jr = 0b001000,
    [Description("jalr")]
    Jalr = 0b001001,

    // System calls
    [Description("syscall")]
    Syscall = 0b001100,
    [Description("break")]
    Break = 0b001101,

    // Move operations
    [Description("mfhi")]
    Mfhi = 0b010000,
    [Description("mthi")]
    Mthi = 0b010001,
    [Description("mflo")]
    Mflo = 0b010010,
    [Description("mtlo")]
    Mtlo = 0b010011,

    // Multiply/Divide
    [Description("mult")]
    Mult = 0b011000,
    [Description("multu")]
    Multu = 0b011001,
    [Description("div")]
    Div = 0b011010,
    [Description("divu")]
    Divu = 0b011011,

    // Arithmetic operations
    [Description("add")]
    Add = 0b100000,
    [Description("addu")]
    Addu = 0b100001,
    [Description("sub")]
    Sub = 0b100010,
    [Description("subu")]
    Subu = 0b100011,

    // Logical operations
    [Description("and")]
    And = 0b100100,
    [Description("or")]
    Or = 0b100101,
    [Description("xor")]
    Xor = 0b100110,
    [Description("nor")]
    Nor = 0b100111,

    // Comparison
    [Description("slt")]
    Slt = 0b101010,
    [Description("sltu")]
    Sltu = 0b101011,
}
