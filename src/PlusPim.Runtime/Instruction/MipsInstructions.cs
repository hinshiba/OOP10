namespace PlusPim.Runtime.Instruction;

/// <summary>
/// MIPSの命令オペコードを表す列挙型
/// </summary>
internal enum MipsOpcode: uint {
    RType = 0b000000,
    // I-Type
    Addi = 0b001000,
    // etc

    // J-Type
    Jump = 0b000010,
    // etc  
}

/// <summary>
/// MIPSのRタイプ命令の機能を表す列挙型
/// </summary>
internal enum MipsRtypeFunction {
    Add = 0b100000,
    // etc
}
