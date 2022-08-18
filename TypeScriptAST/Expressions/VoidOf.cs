using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Expressions;

public class VoidOf : UnaryOperator
{
    internal VoidOf(Expression expression) : base(
        expression,
        OperatorType.VoidOf,
        "void",
        Type.Any) { }

    public override string ToString()
    {
        return $"{Sign} {Operand}";
    }
}
