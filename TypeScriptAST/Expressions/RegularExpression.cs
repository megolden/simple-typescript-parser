namespace TypeScriptAST.Expressions;

public class RegularExpression : Expression
{
    public string Expression { get; private init; }
    public string Flags { get; private init; }

    internal RegularExpression(string expression, string flags) : base(Declarations.Types.Type.RegExp)
    {
        Expression = expression;
        Flags = flags;
    }

    public override string ToString()
    {
        return Expression;
    }
}
