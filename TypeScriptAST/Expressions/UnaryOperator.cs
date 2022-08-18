using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Expressions;

public abstract class UnaryOperator : Operator
{
    protected virtual bool RightToLeftAssociativity { get; } = true;

    public Expression Operand { get; private init; }

    protected UnaryOperator(
        Expression operand,
        OperatorType operatorType,
        string sign,
        Type type,
        params Type[] legalTypes)
        : base(operatorType, sign, type)
    {
        if (legalTypes.Length > 0)
            CheckArgumentType(operand, legalTypes);

        Operand = operand;
    }

    public override string ToString()
    {
        return RightToLeftAssociativity ?
            $"{Sign}{Operand}" :
            $"{Operand}{Sign}";
    }
}
