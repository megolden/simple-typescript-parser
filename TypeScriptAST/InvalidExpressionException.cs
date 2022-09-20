using System;
using TypeScriptAST.Expressions;

namespace TypeScriptAST;

public class InvalidExpressionException : Exception
{
    public Expression Expression { get; }

    public InvalidExpressionException(Expression expression)
        : base($"Invalid expression: {expression}")
    {
        Expression = expression;
    }
}
