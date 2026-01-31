# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is **PlusPim**: a MIPS assembly runtime simulator and debugger written in C# targeting .NET 10.0. The project aims to provide an educational MIPS assembly execution environment with GUI debugger features using Avalonia UI and MVVM architecture.

## Build and Development Commands

### Building the Project
```bash
dotnet build PlusPim.slnx
```

### Code Formatting

**IMPORTANT**: Always apply formatting locally before committing:

```bash
# Apply formatting (use this in local development)
dotnet format PlusPim.slnx
```

The `--verify-no-changes` flag is only for CI to block unformatted PRs. Do NOT use it locally - simply run `dotnet format` to apply formatting automatically.

```bash
# CI verification command (for reference only)
dotnet format --verify-no-changes --verbosity diagnostic PlusPim.slnx
```

### Running the Project
```bash
dotnet run --project src/PlusPim.Runtime/PlusPim.Runtime.csproj
```

## Architecture

### Core Service Pattern

The runtime is organized around a service-oriented architecture with clear separation of concerns:

- **IRuntimeService** (src/PlusPim.Runtime/IRuntimeService.cs): Main orchestrator service
  - Coordinates between Processor, Memory, and Assembler services
  - Handles program loading, execution control, and reset

- **Core Services** (src/PlusPim.Runtime/Services/):
  - **IProcessor**: Executes MIPS instructions step-by-step
  - **IMemory**: Manages the MIPS memory model (text, data, heap, stack segments)
  - **IAssembler**: Converts assembly source to machine code

### MIPS Architecture Implementation

Located in `src/PlusPim.Runtime/MipsArchitecture/`:

- **MipsProcessor**: Implements IProcessor for MIPS instruction execution
- **MipsRegisterFile**: Manages 32 general-purpose registers + PC, HI, LO
- **MipsInstruction.cs**: Defines enums for opcodes (MipsOpcode) and R-type function codes (MipsRtypeFunction)
  - Uses `[Description("mnemonic")]` attributes for instruction name mapping
  - Use `EnumExtensions.TryParseByDescription()` to convert mnemonics to enum values
- **MipsAsembler**: Two-pass assembler implementation
  - **First Pass**: Builds symbol table (labels → addresses)
  - **Second Pass**: Encodes instructions to machine code
  - Comment character: `#` (MIPS standard)
  - Label format: `label_name:` (must be on its own line)
- **MipsInstructionEncoder**: Static utility for encoding R/I/J-type instructions to 32-bit machine code
- **MipsRegisterParser**: Maps register names ($t0, $5, etc.) to register numbers (0-31)
- **EnumExtensions**: Provides Description attribute parsing for enums
- **IMipsInstruction**: Interface for instruction implementations
- **MipsInstructions/**: Directory for individual instruction implementations (e.g., MipsAddi.cs)

### Memory Model

The MIPS memory layout follows standard SPIM conventions:
- Text Segment: `0x00400000` - `0x0FFFFFFF` (256MB, code area)
- Data Segment: `0x10000000` - `0x1FFFFFFF` (256MB, static data)
- Heap: Starting at `0x10040000` (grows upward)
- Stack: Starting at `0x7FFFFFFC` (grows downward, 8MB)

## Supported MIPS Instructions

The simulator targets MIPS I instruction set with:
- Arithmetic: add, addu, addi, addiu, sub, subu, mult, multu, div, divu, mfhi, mflo
- Logical: and, andi, or, ori, xor, xori, nor, sll, srl, sra, sllv, srlv, srav
- Comparison/Branch: slt, slti, sltu, sltiu, beq, bne, blez, bgtz, bltz, bgez
- Jump: j, jal, jr, jalr
- Memory: lw, lh, lhu, lb, lbu, sw, sh, sb, lui
- System: syscall, break, nop
- Pseudo-instructions: li, la, move, b, bnez, beqz, etc.

## Syscall Support

Standard SPIM-compatible syscalls:
- 1: print_int, 4: print_string, 5: read_int
- 8: read_string, 9: sbrk (heap allocation)
- 10: exit, 11: print_char, 12: read_char

## Code Style Conventions

The project follows strict C# formatting rules (enforced via CI):

- **File-scoped namespaces** (no braces for namespace declarations)
- **K&R brace style**: No new line before opening braces
- **Spaces after keywords in control flow disabled**: `if(condition)` not `if (condition)`
- **Primary constructors preferred**: e.g., `RuntimeService(IProcessor processor, IMemory memory)`
- **Explicit this.** qualification required for members
- **No var** - use explicit types
- **Expression-bodied members** for accessors, properties, lambdas
- **LF line endings** (Unix-style)

Check `.editorconfig` for full formatting rules.

## Development Status

Current branch: `feat-runtime` (feature branch for runtime development)

The codebase is in early development:
- Service interfaces are defined
- Basic class stubs exist with NotImplementedException placeholders
- Architecture is planned but implementation is incomplete
- See `doc/要件.md` for detailed requirements (in Japanese)
- See `doc/サービス-ランタイム.puml` and `doc/全体設計.puml` for PlantUML architecture diagrams

## Design Documentation

- `doc/要件.md`: Complete requirements specification (Japanese)
- `doc/*.puml`: PlantUML architecture diagrams
- Architecture images generated from PlantUML are in `doc/*.PNG`
