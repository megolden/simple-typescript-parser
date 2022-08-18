using System;
using System.Linq;
using TypeScriptAST.Expressions;
using Array = TypeScriptAST.Expressions.Array;

namespace TypeScriptAST;

public abstract class ExpressionVisitor
{
    public virtual Expression Visit(Expression node)
    {
        return node switch
        {
            Array array => Visit(array),
            Identifier identifier => Visit(identifier),
            Literal literal => Visit(literal),
            RegularExpression regExp => Visit(regExp),
            TemplateLiteral template => Visit(template),
            Operator op => Visit(op),
            _ => throw new NotSupportedException($"Invalid expression '{node.GetType().Name}'")
        };
    }

    protected virtual Expression Visit(Array node)
    {
        var newItems = node.Items.Select(Visit).ToList();
        return Expression.Array(newItems);
    }

    protected virtual Expression Visit(Identifier node)
    {
        return node;
    }

    protected virtual Expression Visit(Literal node)
    {
        return node;
    }

    protected virtual Expression Visit(RegularExpression node)
    {
        return node;
    }

    protected virtual Expression Visit(TemplateLiteral node)
    {
        var newElements = node.Elements
            .Select(element => element.IsExpression(out var expression) ? Visit(expression) : element)
            .ToList();
        return Expression.TemplateLiteral(newElements);
    }

    protected virtual Expression Visit(Operator node)
    {
        return node switch
        {
            Conditional conditional => Visit(conditional),
            FunctionCall call => Visit(call),
            MemberAccess member => Visit(member),
            MemberIndex index => Visit(index),
            New n => Visit(n),
            BinaryOperator binary => Visit(binary),
            UnaryOperator unary => Visit(unary),
            _ => throw new NotSupportedException($"Invalid operator '{node.GetType().Name}'")
        };
    }

    protected virtual Expression Visit(Conditional node)
    {
        var newCondition = Visit(node.Condition);
        var newTrueExpression = Visit(node.TrueExpression);
        var newFalseExpression = Visit(node.FalseExpression);
        return Expression.Conditional(newCondition, newTrueExpression, newFalseExpression);
    }

    protected virtual Expression Visit(FunctionCall node)
    {
        var newExpression = Visit(node.Expression);
        var newArguments = node.Arguments.Select(Visit).ToList();
        return Expression.FunctionCall(newExpression, newArguments, node.IsOptional);
    }

    protected virtual Expression Visit(MemberAccess node)
    {
        var newInstance = Visit(node.Instance);
        return Expression.MemberAccess(newInstance, node.Member, node.IsOptional);
    }

    protected virtual Expression Visit(MemberIndex node)
    {
        var newInstance = Visit(node.Instance);
        var newIndex = Visit(node.Index);
        return Expression.MemberIndex(newInstance, newIndex, node.IsOptional);
    }

    protected virtual Expression Visit(New node)
    {
        var newArguments = node.Arguments.Select(Visit).ToList();
        return Expression.New(node.Constructor, newArguments);
    }

    protected virtual Expression Visit(BinaryOperator node)
    {
        return node switch
        {
            Add add => Visit(add),
            BitwiseAnd bAnd => Visit(bAnd),
            BitwiseLeftShift leftShift => Visit(leftShift),
            BitwiseOr bOr => Visit(bOr),
            BitwiseRightShift rightShift => Visit(rightShift),
            BitwiseUnsignedRightShift uRightShift => Visit(uRightShift),
            BitwiseXor xor => Visit(xor),
            Divide div => Visit(div),
            Equality eq => Visit(eq),
            Exponent exp => Visit(exp),
            GreaterThan gt => Visit(gt),
            GreaterThanOrEqual gte => Visit(gte),
            Inequality ne => Visit(ne),
            LessThan lt => Visit(lt),
            LessThanOrEqual lte => Visit(lte),
            LogicalAnd lAnd => Visit(lAnd),
            LogicalOr lOr => Visit(lOr),
            Multiply mul => Visit(mul),
            NullishCoalescing nullish => Visit(nullish),
            Remainder rem => Visit(rem),
            StrictEquality seq => Visit(seq),
            StrictInequality sne => Visit(sne),
            Subtract sub => Visit(sub),
            In inOp => Visit(inOp),
            _ => throw new NotSupportedException($"Invalid binary operator '{node.GetType().Name}'")
        };
    }

    protected virtual Expression Visit(UnaryOperator node)
    {
        return node switch
        {
            BitwiseNot bNot => Visit(bNot),
            LogicalNot lNot => Visit(lNot),
            Negate negate => Visit(negate),
            UnaryPlus unaryPlus => Visit(unaryPlus),
            Increment inc => Visit(inc),
            Decrement dec => Visit(dec),
            TypeOf typeOf => Visit(typeOf),
            VoidOf voidOf => Visit(voidOf),
            As asOp => Visit(asOp),
            InstanceOf instance => Visit(instance),
            _ => throw new NotSupportedException($"Invalid unary operator '{node.GetType().Name}'")
        };
    }

    protected virtual Expression Visit(BitwiseNot node)
    {
        var newOperand = Visit(node.Operand);
        return Expression.BitwiseNot(newOperand);
    }

    protected virtual Expression Visit(LogicalNot node)
    {
        var newOperand = Visit(node.Operand);
        return Expression.LogicalNot(newOperand);
    }

    protected virtual Expression Visit(Negate node)
    {
        var newOperand = Visit(node.Operand);
        return Expression.Negate(newOperand);
    }

    protected virtual Expression Visit(Increment node)
    {
        var newOperand = Visit(node.Operand);
        return node.IsPrefix ?
            Expression.PrefixIncrement(newOperand):
            Expression.PostfixIncrement(newOperand);
    }

    protected virtual Expression Visit(Decrement node)
    {
        var newOperand = Visit(node.Operand);
        return node.IsPrefix ?
            Expression.PrefixDecrement(newOperand):
            Expression.PostfixDecrement(newOperand);
    }

    protected virtual Expression Visit(UnaryPlus node)
    {
        var newOperand = Visit(node.Operand);
        return Expression.UnaryPlus(newOperand);
    }

    protected virtual Expression Visit(TypeOf node)
    {
        var newOperand = Visit(node.Operand);
        return Expression.TypeOf(newOperand);
    }

    protected virtual Expression Visit(VoidOf node)
    {
        var newOperand = Visit(node.Operand);
        return Expression.VoidOf(newOperand);
    }

    protected virtual Expression Visit(As node)
    {
        var newOperand = Visit(node.Operand);
        return Expression.As(newOperand, node.TargetType);
    }

    protected virtual Expression Visit(InstanceOf node)
    {
        var newOperand = Visit(node.Operand);
        return Expression.InstanceOf(newOperand, node.TargetType);
    }

    protected virtual Expression Visit(Add node)
    {
        var newLeft = Visit(node.Left);
        var newRight = Visit(node.Right);
        return Expression.Add(newLeft, newRight);
    }

    protected virtual Expression Visit(BitwiseAnd node)
    {
        var newLeft = Visit(node.Left);
        var newRight = Visit(node.Right);
        return Expression.BitwiseAnd(newLeft, newRight);
    }

    protected virtual Expression Visit(BitwiseLeftShift node)
    {
        var newLeft = Visit(node.Left);
        var newRight = Visit(node.Right);
        return Expression.BitwiseLeftShift(newLeft, newRight);
    }

    protected virtual Expression Visit(BitwiseOr node)
    {
        var newLeft = Visit(node.Left);
        var newRight = Visit(node.Right);
        return Expression.BitwiseOr(newLeft, newRight);
    }

    protected virtual Expression Visit(BitwiseRightShift node)
    {
        var newLeft = Visit(node.Left);
        var newRight = Visit(node.Right);
        return Expression.BitwiseRightShift(newLeft, newRight);
    }

    protected virtual Expression Visit(BitwiseUnsignedRightShift node)
    {
        var newLeft = Visit(node.Left);
        var newRight = Visit(node.Right);
        return Expression.BitwiseUnsignedRightShift(newLeft, newRight);
    }

    protected virtual Expression Visit(BitwiseXor node)
    {
        var newLeft = Visit(node.Left);
        var newRight = Visit(node.Right);
        return Expression.BitwiseXor(newLeft, newRight);
    }

    protected virtual Expression Visit(Divide node)
    {
        var newLeft = Visit(node.Left);
        var newRight = Visit(node.Right);
        return Expression.Divide(newLeft, newRight);
    }

    protected virtual Expression Visit(Equality node)
    {
        var newLeft = Visit(node.Left);
        var newRight = Visit(node.Right);
        return Expression.Equality(newLeft, newRight);
    }

    protected virtual Expression Visit(Exponent node)
    {
        var newLeft = Visit(node.Left);
        var newRight = Visit(node.Right);
        return Expression.Exponentiation(newLeft, newRight);
    }

    protected virtual Expression Visit(GreaterThan node)
    {
        var newLeft = Visit(node.Left);
        var newRight = Visit(node.Right);
        return Expression.GreaterThan(newLeft, newRight);
    }

    protected virtual Expression Visit(GreaterThanOrEqual node)
    {
        var newLeft = Visit(node.Left);
        var newRight = Visit(node.Right);
        return Expression.GreaterThanOrEqual(newLeft, newRight);
    }

    protected virtual Expression Visit(Inequality node)
    {
        var newLeft = Visit(node.Left);
        var newRight = Visit(node.Right);
        return Expression.Inequality(newLeft, newRight);
    }

    protected virtual Expression Visit(LessThan node)
    {
        var newLeft = Visit(node.Left);
        var newRight = Visit(node.Right);
        return Expression.LessThan(newLeft, newRight);
    }

    protected virtual Expression Visit(LessThanOrEqual node)
    {
        var newLeft = Visit(node.Left);
        var newRight = Visit(node.Right);
        return Expression.LessThanOrEqual(newLeft, newRight);
    }

    protected virtual Expression Visit(LogicalAnd node)
    {
        var newLeft = Visit(node.Left);
        var newRight = Visit(node.Right);
        return Expression.LogicalAnd(newLeft, newRight);
    }

    protected virtual Expression Visit(LogicalOr node)
    {
        var newLeft = Visit(node.Left);
        var newRight = Visit(node.Right);
        return Expression.LogicalOr(newLeft, newRight);
    }

    protected virtual Expression Visit(Multiply node)
    {
        var newLeft = Visit(node.Left);
        var newRight = Visit(node.Right);
        return Expression.Multiply(newLeft, newRight);
    }

    protected virtual Expression Visit(NullishCoalescing node)
    {
        var newLeft = Visit(node.Left);
        var newRight = Visit(node.Right);
        return Expression.NullishCoalescing(newLeft, newRight);
    }

    protected virtual Expression Visit(Remainder node)
    {
        var newLeft = Visit(node.Left);
        var newRight = Visit(node.Right);
        return Expression.Remainder(newLeft, newRight);
    }

    protected virtual Expression Visit(StrictEquality node)
    {
        var newLeft = Visit(node.Left);
        var newRight = Visit(node.Right);
        return Expression.StrictEquality(newLeft, newRight);
    }

    protected virtual Expression Visit(StrictInequality node)
    {
        var newLeft = Visit(node.Left);
        var newRight = Visit(node.Right);
        return Expression.StrictInequality(newLeft, newRight);
    }

    protected virtual Expression Visit(Subtract node)
    {
        var newLeft = Visit(node.Left);
        var newRight = Visit(node.Right);
        return Expression.Subtract(newLeft, newRight);
    }

    protected virtual Expression Visit(In node)
    {
        var newLeft = Visit(node.Left);
        var newRight = Visit(node.Right);
        return Expression.In(newLeft, newRight);
    }
}
