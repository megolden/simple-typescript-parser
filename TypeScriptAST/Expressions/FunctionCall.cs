using System.Collections.Generic;
using System.Linq;
using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Expressions;

public class FunctionCall : Operator
{
    public Expression Expression { get; }
    public IReadOnlyList<Expression> Arguments { get; }
    public bool IsOptional { get; }

    internal FunctionCall(Expression expression, IEnumerable<Expression> arguments, bool isOptional)
        : this(expression, arguments, expression.Type, isOptional) { }
    internal FunctionCall(Expression expression, IEnumerable<Expression> arguments, Type type, bool isOptional)
        : base(OperatorType.FunctionCall, "()", type)
    {
        Expression = expression;
        IsOptional = isOptional;
        Arguments = arguments.ToList();
    }

    public override string ToString()
    {
        return $"{Expression}{(IsOptional ? "?." : "")}({System.String.Join(", ", Arguments)})";
    }
}
