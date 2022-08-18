namespace TypeScriptAST.Expressions;

public class Conditional : Operator
{
    public Expression Condition { get; private init; }
    public Expression TrueExpression { get; private init; }
    public Expression FalseExpression { get; private init; }

    internal Conditional(Expression condition, Expression trueExpression, Expression falseExpression)
        : base(OperatorType.ConditionalTernary, "?:", trueExpression.Type)
    {
        CheckArgumentType(falseExpression, trueExpression.Type);

        Condition = condition;
        TrueExpression = trueExpression;
        FalseExpression = falseExpression;
    }

    public override string ToString()
    {
        return $"{Condition} ? {TrueExpression} : {FalseExpression}";
    }
}
