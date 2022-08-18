using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Expressions;

public class TypeOf : UnaryOperator
{
    internal TypeOf(Expression expression) : base(
        expression,
        OperatorType.TypeOf,
        "typeof",
        Type.String) { }

    public override string ToString()
    {
        return $"{Sign} {Operand}";
    }
}
