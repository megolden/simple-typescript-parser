using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Expressions;

public class Increment : UnaryOperator
{
    protected override bool RightToLeftAssociativity => IsPrefix;
    public bool IsPrefix { get; private init; }

    internal Increment(Expression operand, bool isPrefix) : base(
        operand,
        isPrefix ? OperatorType.PrefixIncrement : OperatorType.PostfixIncrement,
        "++",
        operand.Type,
        Type.Number)
    {
        IsPrefix = isPrefix;
    }
}
