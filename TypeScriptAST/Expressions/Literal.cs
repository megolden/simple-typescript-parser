using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Expressions;

public class Literal : Expression
{
    public object? Value { get; private init; }

    internal Literal(object? value, Type type) : base(type)
    {
        Value = value;
    }

    public override string? ToString()
    {
        return Value switch
        {
            null => "null",
            string str => '"' + str + '"',
            var other => other.ToString()
        };
    }
}
