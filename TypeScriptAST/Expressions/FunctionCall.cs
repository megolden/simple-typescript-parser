using System;
using System.Collections.Generic;
using System.Linq;

namespace TypeScriptAST.Expressions;

public class FunctionCall : Operator
{
    public Expression Expression { get; private init; }
    public IReadOnlyList<Expression> Arguments { get; private init; }
    public bool IsOptional { get; private init; }

    internal FunctionCall(Expression expression, IEnumerable<Expression> arguments, bool isOptional)
        : base(OperatorType.FunctionCall, "()", expression.Type)
    {
        Expression = expression;
        IsOptional = isOptional;
        Arguments = arguments.ToList();
    }

    public override string ToString()
    {
        return $"{Expression}{(IsOptional ? "?." : "")}({String.Join(", ", Arguments)})";
    }
}
