using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Expressions;

public class Decrement : UnaryOperator
{
    protected override bool RightToLeftAssociativity => IsPrefix;
    public bool IsPrefix { get; private init; }

    internal Decrement(Expression operand, bool isPrefix) : base(
        operand,
        isPrefix ? OperatorType.PrefixDecrement : OperatorType.PostfixDecrement,
        "--",
        operand.Type,
        Type.Number)
    {
        IsPrefix = isPrefix;
    }
}
