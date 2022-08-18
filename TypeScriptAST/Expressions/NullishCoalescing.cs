namespace TypeScriptAST.Expressions;

public class NullishCoalescing : BinaryOperator
{
    internal NullishCoalescing(Expression left, Expression right) : base(
        left,
        right,
        OperatorType.NullishCoalescing,
        "??",
        left.Type)
    {
        CheckArgumentType(right, left.Type);
    }
}
