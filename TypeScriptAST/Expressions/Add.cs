using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Expressions;

public class Add : BinaryOperator
{
    internal Add(Expression left, Expression right) : base(
        left,
        right,
        OperatorType.Addition,
        "+",
        ResolveType(left, right)) { }

    private static Type ResolveType(Expression left, Expression right)
    {
        if (left.Type == Type.String || right.Type == Type.String)
            return Type.String;

        if (left.Type == Type.Number && right.Type == Type.Number)
            return Type.Number;

        throw new InvalidOperandException(OperatorType.Addition, right);
    }
}
