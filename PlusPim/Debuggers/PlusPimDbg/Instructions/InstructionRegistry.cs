using PlusPim.Debuggers.PlusPimDbg.Instructions.RType;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace PlusPim.Debuggers.PlusPimDbg.Instructions;

internal sealed partial class InstructionRegistry {
    [GeneratedRegex(@"^(?<op>\w+)\s+(?<operands>.+)$")]
    private static partial Regex AssemblyLinePattern();

    public static InstructionRegistry Default => field ??= CreateDefault();

    private readonly Dictionary<string, IInstructionParser> _parsers;

    private InstructionRegistry(Dictionary<string, IInstructionParser> parsers) {
        this._parsers = parsers;
    }

    public static InstructionRegistry CreateDefault() {
        Dictionary<string, IInstructionParser> parsers = new(StringComparer.OrdinalIgnoreCase);

        RegisterParser(parsers, new AddInstructionParser());
        RegisterParser(parsers, new AdduInstructionParser());
        RegisterParser(parsers, new SubInstructionParser());

        return new InstructionRegistry(parsers);
    }

    private static void RegisterParser(Dictionary<string, IInstructionParser> parsers, IInstructionParser parser) {
        parsers[parser.Mnemonic] = parser;
    }

    public bool TryParse(string assemblyLine, [MaybeNullWhen(false)] out IInstruction instruction) {
        instruction = null;

        Match match = AssemblyLinePattern().Match(assemblyLine);
        if(!match.Success) {
            return false;
        }

        string op = match.Groups["op"].Value;
        string operands = match.Groups["operands"].Value;

        return this._parsers.TryGetValue(op, out IInstructionParser? parser) && parser.TryParse(operands, out instruction);
    }
}
