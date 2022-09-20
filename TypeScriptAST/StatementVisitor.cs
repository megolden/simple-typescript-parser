using System;
using System.Linq;
using TypeScriptAST.Declarations;
using TypeScriptAST.Expressions;
using Array = TypeScriptAST.Expressions.Array;

namespace TypeScriptAST;

public abstract class StatementVisitor
{
    public virtual Statement Visit(Statement node)
    {
        return node switch
        {
            Expression expression => Visit(expression),
            StatementList list => Visit(list),
            EmptyStatement empty => Visit(empty),
            Comment comment => Visit(comment),
            Declaration declaration => Visit(declaration),
            _ => throw new NotSupportedException($"Invalid statement '{node.GetType().Name}'")
        };
    }

    protected virtual Statement Visit(Expression node)
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

    protected virtual Statement Visit(StatementList node)
    {
        var newStatements = node.Select(Visit).ToList();
        return Statement.List(newStatements);
    }

    protected virtual Statement Visit(EmptyStatement node)
    {
        return node;
    }

    protected virtual Statement Visit(Comment node)
    {
        return node switch
        {
            XmlComment xml => Visit(xml),
            _ => node
        };
    }

    protected virtual Statement Visit(XmlComment node)
    {
        return node;
    }

    protected virtual Statement Visit(Declaration node)
    {
        return node;
    }

    protected virtual Statement Visit(Array node)
    {
        var newItems = node.Items.Select(Visit).Cast<Expression>().ToList();
        return Expression.Array(newItems);
    }

    protected virtual Statement Visit(Identifier node)
    {
        return node;
    }

    protected virtual Statement Visit(Literal node)
    {
        return node;
    }

    protected virtual Statement Visit(RegularExpression node)
    {
        return node;
    }

    protected virtual Statement Visit(TemplateLiteral node)
    {
        var newElements = node.Elements
            .Select(element => element.IsExpression(out var expression) ? (Expression)Visit(expression) : element)
            .ToList();
        return Expression.TemplateLiteral(newElements);
    }

    protected virtual Statement Visit(Operator node)
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

    protected virtual Statement Visit(Conditional node)
    {
        var newCondition = (Expression)Visit(node.Condition);
        var newTrueExpression = (Expression)Visit(node.TrueExpression);
        var newFalseExpression = (Expression)Visit(node.FalseExpression);
        return Expression.Conditional(newCondition, newTrueExpression, newFalseExpression);
    }

    protected virtual Statement Visit(FunctionCall node)
    {
        var newExpression = (Expression)Visit(node.Expression);
        var newArguments = node.Arguments.Select(Visit).Cast<Expression>().ToList();
        return Expression.FunctionCall(newExpression, newArguments, node.IsOptional);
    }

    protected virtual Statement Visit(MemberAccess node)
    {
        var newInstance = (Expression)Visit(node.Instance);
        return Expression.MemberAccess(newInstance, node.Member, node.IsOptional);
    }

    protected virtual Statement Visit(MemberIndex node)
    {
        var newInstance = (Expression)Visit(node.Instance);
        var newIndex = (Expression)Visit(node.Index);
        return Expression.MemberIndex(newInstance, newIndex, node.IsOptional);
    }

    protected virtual Statement Visit(New node)
    {
        var newArguments = node.Arguments.Select(Visit).Cast<Expression>().ToList();
        return Expression.New(node.Constructor, newArguments);
    }

    protected virtual Statement Visit(BinaryOperator node)
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

    protected virtual Statement Visit(UnaryOperator node)
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

    protected virtual Statement Visit(BitwiseNot node)
    {
        var newOperand = (Expression)Visit(node.Operand);
        return Expression.BitwiseNot(newOperand);
    }

    protected virtual Statement Visit(LogicalNot node)
    {
        var newOperand = (Expression)Visit(node.Operand);
        return Expression.LogicalNot(newOperand);
    }

    protected virtual Statement Visit(Negate node)
    {
        var newOperand = (Expression)Visit(node.Operand);
        return Expression.Negate(newOperand);
    }

    protected virtual Statement Visit(Increment node)
    {
        var newOperand = (Expression)Visit(node.Operand);
        return node.IsPrefix ?
            Expression.PrefixIncrement(newOperand):
            Expression.PostfixIncrement(newOperand);
    }

    protected virtual Statement Visit(Decrement node)
    {
        var newOperand = (Expression)Visit(node.Operand);
        return node.IsPrefix ?
            Expression.PrefixDecrement(newOperand):
            Expression.PostfixDecrement(newOperand);
    }

    protected virtual Statement Visit(UnaryPlus node)
    {
        var newOperand = (Expression)Visit(node.Operand);
        return Expression.UnaryPlus(newOperand);
    }

    protected virtual Statement Visit(TypeOf node)
    {
        var newOperand = (Expression)Visit(node.Operand);
        return Expression.TypeOf(newOperand);
    }

    protected virtual Statement Visit(VoidOf node)
    {
        var newOperand = (Expression)Visit(node.Operand);
        return Expression.VoidOf(newOperand);
    }

    protected virtual Statement Visit(As node)
    {
        var newOperand = (Expression)Visit(node.Operand);
        return Expression.As(newOperand, node.TargetType);
    }

    protected virtual Statement Visit(InstanceOf node)
    {
        var newOperand = (Expression)Visit(node.Operand);
        var newTargetType = (Expression)Visit(node.TargetType);
        return Expression.InstanceOf(newOperand, newTargetType);
    }

    protected virtual Statement Visit(Add node)
    {
        var newLeft = (Expression)Visit(node.Left);
        var newRight = (Expression)Visit(node.Right);
        return Expression.Add(newLeft, newRight);
    }

    protected virtual Statement Visit(BitwiseAnd node)
    {
        var newLeft = (Expression)Visit(node.Left);
        var newRight = (Expression)Visit(node.Right);
        return Expression.BitwiseAnd(newLeft, newRight);
    }

    protected virtual Statement Visit(BitwiseLeftShift node)
    {
        var newLeft = (Expression)Visit(node.Left);
        var newRight = (Expression)Visit(node.Right);
        return Expression.BitwiseLeftShift(newLeft, newRight);
    }

    protected virtual Statement Visit(BitwiseOr node)
    {
        var newLeft = (Expression)Visit(node.Left);
        var newRight = (Expression)Visit(node.Right);
        return Expression.BitwiseOr(newLeft, newRight);
    }

    protected virtual Statement Visit(BitwiseRightShift node)
    {
        var newLeft = (Expression)Visit(node.Left);
        var newRight = (Expression)Visit(node.Right);
        return Expression.BitwiseRightShift(newLeft, newRight);
    }

    protected virtual Statement Visit(BitwiseUnsignedRightShift node)
    {
        var newLeft = (Expression)Visit(node.Left);
        var newRight = (Expression)Visit(node.Right);
        return Expression.BitwiseUnsignedRightShift(newLeft, newRight);
    }

    protected virtual Statement Visit(BitwiseXor node)
    {
        var newLeft = (Expression)Visit(node.Left);
        var newRight = (Expression)Visit(node.Right);
        return Expression.BitwiseXor(newLeft, newRight);
    }

    protected virtual Statement Visit(Divide node)
    {
        var newLeft = (Expression)Visit(node.Left);
        var newRight = (Expression)Visit(node.Right);
        return Expression.Divide(newLeft, newRight);
    }

    protected virtual Statement Visit(Equality node)
    {
        var newLeft = (Expression)Visit(node.Left);
        var newRight = (Expression)Visit(node.Right);
        return Expression.Equality(newLeft, newRight);
    }

    protected virtual Statement Visit(Exponent node)
    {
        var newLeft = (Expression)Visit(node.Left);
        var newRight = (Expression)Visit(node.Right);
        return Expression.Exponentiation(newLeft, newRight);
    }

    protected virtual Statement Visit(GreaterThan node)
    {
        var newLeft = (Expression)Visit(node.Left);
        var newRight = (Expression)Visit(node.Right);
        return Expression.GreaterThan(newLeft, newRight);
    }

    protected virtual Statement Visit(GreaterThanOrEqual node)
    {
        var newLeft = (Expression)Visit(node.Left);
        var newRight = (Expression)Visit(node.Right);
        return Expression.GreaterThanOrEqual(newLeft, newRight);
    }

    protected virtual Statement Visit(Inequality node)
    {
        var newLeft = (Expression)Visit(node.Left);
        var newRight = (Expression)Visit(node.Right);
        return Expression.Inequality(newLeft, newRight);
    }

    protected virtual Statement Visit(LessThan node)
    {
        var newLeft = (Expression)Visit(node.Left);
        var newRight = (Expression)Visit(node.Right);
        return Expression.LessThan(newLeft, newRight);
    }

    protected virtual Statement Visit(LessThanOrEqual node)
    {
        var newLeft = (Expression)Visit(node.Left);
        var newRight = (Expression)Visit(node.Right);
        return Expression.LessThanOrEqual(newLeft, newRight);
    }

    protected virtual Statement Visit(LogicalAnd node)
    {
        var newLeft = (Expression)Visit(node.Left);
        var newRight = (Expression)Visit(node.Right);
        return Expression.LogicalAnd(newLeft, newRight);
    }

    protected virtual Statement Visit(LogicalOr node)
    {
        var newLeft = (Expression)Visit(node.Left);
        var newRight = (Expression)Visit(node.Right);
        return Expression.LogicalOr(newLeft, newRight);
    }

    protected virtual Statement Visit(Multiply node)
    {
        var newLeft = (Expression)Visit(node.Left);
        var newRight = (Expression)Visit(node.Right);
        return Expression.Multiply(newLeft, newRight);
    }

    protected virtual Statement Visit(NullishCoalescing node)
    {
        var newLeft = (Expression)Visit(node.Left);
        var newRight = (Expression)Visit(node.Right);
        return Expression.NullishCoalescing(newLeft, newRight);
    }

    protected virtual Statement Visit(Remainder node)
    {
        var newLeft = (Expression)Visit(node.Left);
        var newRight = (Expression)Visit(node.Right);
        return Expression.Remainder(newLeft, newRight);
    }

    protected virtual Statement Visit(StrictEquality node)
    {
        var newLeft = (Expression)Visit(node.Left);
        var newRight = (Expression)Visit(node.Right);
        return Expression.StrictEquality(newLeft, newRight);
    }

    protected virtual Statement Visit(StrictInequality node)
    {
        var newLeft = (Expression)Visit(node.Left);
        var newRight = (Expression)Visit(node.Right);
        return Expression.StrictInequality(newLeft, newRight);
    }

    protected virtual Statement Visit(Subtract node)
    {
        var newLeft = (Expression)Visit(node.Left);
        var newRight = (Expression)Visit(node.Right);
        return Expression.Subtract(newLeft, newRight);
    }

    protected virtual Statement Visit(In node)
    {
        var newLeft = (Expression)Visit(node.Left);
        var newRight = (Expression)Visit(node.Right);
        return Expression.In(newLeft, newRight);
    }
}
