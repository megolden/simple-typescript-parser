using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Expressions;

public abstract class BinaryOperator : Operator
{
    public Expression Left { get; private init; }
    public Expression Right { get; private init; }

    protected BinaryOperator(
        Expression left,
        Expression right,
        OperatorType operatorType,
        string sign,
        Type type,
        params Type[] legalTypes)
        : base(operatorType, sign, type)
    {
        if (legalTypes.Length > 0)
        {
            CheckArgumentType(left, legalTypes);
            CheckArgumentType(right, legalTypes);
        }

        Left = left;
        Right = right;
    }

    public override string ToString()
    {
        return $"{Left} {Sign} {Right}";
    }
}
