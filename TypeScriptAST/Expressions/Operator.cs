using System.Linq;
using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Expressions;

public abstract class Operator : Expression
{
    public OperatorType OperatorType { get; }
    public string Sign { get; }

    protected Operator(OperatorType operatorType, string sign, Type type)
        : base(type)
    {
        Sign = sign;
        OperatorType = operatorType;
    }

    protected void CheckArgumentType(Expression operand, params Type[] legalTypes)
    {
        if (!legalTypes.Any(_ => _.IsAssignableFrom(operand.Type)))
            throw new InvalidOperandException(OperatorType, operand);
    }
}
