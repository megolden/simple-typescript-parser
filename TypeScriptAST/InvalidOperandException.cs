using System;
using TypeScriptAST.Expressions;

namespace TypeScriptAST;

public class InvalidOperandException : Exception
{
    public OperatorType OperatorType { get; }
    public Expression Operand { get; }

    public InvalidOperandException(OperatorType operatorType, Expression operand)
        : base($"Invalid operand for operator {operatorType}: {operand}")
    {
        OperatorType = operatorType;
        Operand = operand;
    }
}
