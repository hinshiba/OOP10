# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

PlusPim is a MIPS assembly debugger with VS Code integration. It consists of:
- A .NET 10 debug adapter that implements the VS Code Debug Adapter Protocol (DAP)
- A VS Code extension that integrates with the debug adapter
- MIPS assembly parsing and execution infrastructure

## Build Commands

```bash
# Build the .NET solution (from repository root)
dotnet build PlusPim.slnx

# Build the VS Code extension (from vscode_ext/pluspim)
pnpm install
pnpm run compile

# Watch mode for extension development
pnpm run watch

# Lint the extension
pnpm run lint
```

## Architecture

The project follows a layered architecture with dependency inversion:

```
EditorController (DebugAdapter) → IApplication ← Application → IDebugger ← Debuggers
```

### Key Components

**PlusPim/** - Main debug adapter executable
- `Program.cs` - Entry point, wires up DebugAdapter, Application, and PlusPimDbg
- `Application/` - Core application logic implementing `IApplication`
- `EditorController/DebugAdapter/` - DAP implementation using `Microsoft.VisualStudio.Shared.VSCodeDebugProtocol`
- `Debuggers/PlusPimDbg/` - MIPS debugger implementation
  - `Mnemonic.cs` - MIPS instruction parsing (add, addu, sub, etc.)
  - `ParsedProgram.cs` - Source file parsing with label/symbol table support

**vscode_ext/pluspim/** - VS Code extension
- Registers the `pluspim` debug type
- Launches `PlusPim.exe` as the debug adapter

**src/PlusPim.Runtime/** - Shared runtime library (currently minimal)

### Interfaces

- `IApplication` - Application layer interface for debugger operations (Load, etc.)
- `IDebugger` - Debugger abstraction allowing different backends (PlusPimDbg, SpimGateway planned)

### MIPS Register Model

The `Register` enum defines all 32 MIPS registers (zero, at, v0-v1, a0-a3, t0-t9, s0-s8, k0-k1, gp, sp, ra).

## Design Reference

See `doc/おおまかな設計.puml` for the PlantUML architecture diagram showing planned interfaces and domain models.
