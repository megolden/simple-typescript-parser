using System.Collections.Generic;
using TypeScriptAST.Declarations;
using TypeScriptAST.Declarations.Types;
using TemplateElement = TypeScriptAST.Expressions.TemplateLiteral.TemplateElement;

namespace TypeScriptAST.Expressions;

public abstract class Expression
{
    public Type Type { get; private init; }

    protected Expression(Type type)
    {
        Type = type;
    }

    public static Literal Literal(object? value, Type type)
    {
        return new Literal(value, type);
    }

    public static TemplateLiteral TemplateLiteral(IEnumerable<TemplateElement> elements)
    {
        return new TemplateLiteral(elements);
    }

    public static RegularExpression RegularExpression(string expression, string flags)
    {
        return new RegularExpression(expression, flags);
    }

    public static Identifier Identifier(IMemberInfo subject)
    {
        return new Identifier(subject);
    }

    public static Add Add(Expression left, Expression right)
    {
        return new Add(left, right);
    }

    public static Subtract Subtract(Expression left, Expression right)
    {
        return new Subtract(left, right);
    }

    public static Multiply Multiply(Expression left, Expression right)
    {
        return new Multiply(left, right);
    }

    public static Divide Divide(Expression left, Expression right)
    {
        return new Divide(left, right);
    }

    public static Remainder Remainder(Expression left, Expression right)
    {
        return new Remainder(left, right);
    }

    public static Exponent Exponentiation(Expression left, Expression right)
    {
        return new Exponent(left, right);
    }

    public static Negate Negate(Expression operand)
    {
        return new Negate(operand);
    }

    public static LogicalNot LogicalNot(Expression operand)
    {
        return new LogicalNot(operand);
    }

    public static LogicalAnd LogicalAnd(Expression left, Expression right)
    {
        return new LogicalAnd(left, right);
    }

    public static LogicalOr LogicalOr(Expression left, Expression right)
    {
        return new LogicalOr(left, right);
    }

    public static UnaryPlus UnaryPlus(Expression operand)
    {
        return new UnaryPlus(operand);
    }

    public static BitwiseNot BitwiseNot(Expression operand)
    {
        return new BitwiseNot(operand);
    }

    public static BitwiseAnd BitwiseAnd(Expression left, Expression right)
    {
        return new BitwiseAnd(left, right);
    }

    public static BitwiseOr BitwiseOr(Expression left, Expression right)
    {
        return new BitwiseOr(left, right);
    }

    public static BitwiseXor BitwiseXor(Expression left, Expression right)
    {
        return new BitwiseXor(left, right);
    }

    public static BitwiseLeftShift BitwiseLeftShift(Expression left, Expression right)
    {
        return new BitwiseLeftShift(left, right);
    }

    public static BitwiseRightShift BitwiseRightShift(Expression left, Expression right)
    {
        return new BitwiseRightShift(left, right);
    }

    public static BitwiseUnsignedRightShift BitwiseUnsignedRightShift(Expression left, Expression right)
    {
        return new BitwiseUnsignedRightShift(left, right);
    }

    public static LessThan LessThan(Expression left, Expression right)
    {
        return new LessThan(left, right);
    }

    public static LessThanOrEqual LessThanOrEqual(Expression left, Expression right)
    {
        return new LessThanOrEqual(left, right);
    }

    public static GreaterThan GreaterThan(Expression left, Expression right)
    {
        return new GreaterThan(left, right);
    }

    public static GreaterThanOrEqual GreaterThanOrEqual(Expression left, Expression right)
    {
        return new GreaterThanOrEqual(left, right);
    }

    public static Equality Equality(Expression left, Expression right)
    {
        return new Equality(left, right);
    }

    public static Inequality Inequality(Expression left, Expression right)
    {
        return new Inequality(left, right);
    }

    public static StrictEquality StrictEquality(Expression left, Expression right)
    {
        return new StrictEquality(left, right);
    }

    public static StrictInequality StrictInequality(Expression left, Expression right)
    {
        return new StrictInequality(left, right);
    }

    public static NullishCoalescing NullishCoalescing(Expression left, Expression right)
    {
        return new NullishCoalescing(left, right);
    }

    public static Conditional Conditional(Expression condition, Expression trueExpression, Expression falseExpression)
    {
        return new Conditional(condition, trueExpression, falseExpression);
    }

    public static MemberAccess MemberAccess(Expression instance, IMemberInfo member, bool isOptional)
    {
        return new MemberAccess(instance, member, isOptional);
    }

    public static MemberIndex MemberIndex(Expression instance, Expression index, bool isOptional)
    {
        return new MemberIndex(instance, index, isOptional);
    }

    public static FunctionCall FunctionCall(Expression expression, IEnumerable<Expression> arguments, bool isOptional)
    {
        return new FunctionCall(expression, arguments, isOptional);
    }

    public static Increment PrefixIncrement(Expression operand)
    {
        return new Increment(operand, isPrefix: true);
    }

    public static Decrement PrefixDecrement(Expression operand)
    {
        return new Decrement(operand, isPrefix: true);
    }

    public static Increment PostfixIncrement(Expression operand)
    {
        return new Increment(operand, isPrefix: false);
    }

    public static Decrement PostfixDecrement(Expression operand)
    {
        return new Decrement(operand, isPrefix: false);
    }

    public static New New(FunctionDefinition constructor, IEnumerable<Expression> arguments)
    {
        return new New(constructor, arguments);
    }

    public static Array Array(IEnumerable<Expression> items)
    {
        return new Array(items);
    }

    public static InstanceOf InstanceOf(Expression obj, Type targetType)
    {
        return new InstanceOf(obj, targetType);
    }

    public static In In(Expression property, Expression obj)
    {
        return new In(property, obj);
    }

    public static As As(Expression value, Type targetType)
    {
        return new As(value, targetType);
    }

    public static VoidOf VoidOf(Expression expression)
    {
        return new VoidOf(expression);
    }

    public static TypeOf TypeOf(Expression expression)
    {
        return new TypeOf(expression);
    }
}
