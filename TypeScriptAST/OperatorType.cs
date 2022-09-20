using System;

namespace TypeScriptAST;

public class OperatorType
{
    public string Name { get; }
    public int Precedence { get; }

    private OperatorType(string name, int precedence)
    {
        Name = name;
        Precedence = precedence;
    }

    public override string ToString()
    {
        return Name;
    }

    public override bool Equals(object? obj)
    {
        return obj is OperatorType that && that.Name == this.Name;
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }

    public static bool operator ==(OperatorType? op1, OperatorType? op2)
    {
        return Object.Equals(op1, op2);
    }

    public static bool operator !=(OperatorType? op1, OperatorType? op2)
    {
        return !(op1 == op2);
    }

    public static OperatorType? FromToken(Token token, bool hasLeftOperand)
    {
        return token.Type switch
        {
            TokenType.Minus => hasLeftOperand ? Subtraction : UnaryNegation,
            TokenType.Plus => hasLeftOperand ? Addition : UnaryPlus,
            TokenType.Asterisk => Multiplication,
            TokenType.Slash => Division,
            TokenType.Percent => Remainder,
            TokenType.DoubleAsterisk => Exponentiation,
            TokenType.Exclamation => LogicalNot,
            TokenType.DoubleAmpersand => LogicalAnd,
            TokenType.DoubleBar => LogicalOr,
            TokenType.Tilde => BitwiseNot,
            TokenType.Carat => BitwiseXor,
            TokenType.Bar => BitwiseOr,
            TokenType.Ampersand => BitwiseAnd,
            TokenType.DoubleLessThan => BitwiseLeftShift,
            TokenType.DoubleGreaterThan => BitwiseRightShift,
            TokenType.TripleGreaterThan => BitwiseUnsignedRightShift,
            TokenType.LessThan => LessThan,
            TokenType.LessThanEqual => LessThanOrEqual,
            TokenType.GreaterThan => GreaterThan,
            TokenType.GreaterThanEqual => GreaterThanOrEqual,
            TokenType.DoubleEqual => Equality,
            TokenType.ExclamationEqual => Inequality,
            TokenType.TripleEqual => StrictEquality,
            TokenType.ExclamationDoubleEqual => StrictInequality,
            TokenType.DoubleQuestion => NullishCoalescing,
            TokenType.Question or TokenType.Colon => ConditionalTernary,
            TokenType.OpenParen or TokenType.OpenBracket => Grouping,
            TokenType.Dot => MemberAccess,
            TokenType.QuestionDot => OptionalMemberAccess,
            TokenType.Comma => Comma,
            TokenType.DoublePlus => hasLeftOperand ? PostfixIncrement : PrefixIncrement,
            TokenType.DoubleMinus => hasLeftOperand ? PostfixDecrement : PrefixDecrement,
            TokenType.Identifier when token.Text == "as" => As,
            TokenType.Identifier when token.Text == "instanceof" => InstanceOf,
            TokenType.Identifier when token.Text == "in" => In,
            TokenType.Identifier when token.Text == "void" => VoidOf,
            TokenType.Identifier when token.Text == "typeof" => TypeOf,
            _ => null
        };
    }

    public static readonly OperatorType Grouping = new(nameof(Grouping), precedence: 21);                                         // (x)
    public static readonly OperatorType MemberAccess = new(nameof(MemberAccess), precedence: 20);                                 // x.y
    public static readonly OperatorType New = new(nameof(New), precedence: 20);                                                   // new x(...)
    public static readonly OperatorType FunctionCall = new(nameof(FunctionCall), precedence: 20);                                 // x(...)
    public static readonly OperatorType OptionalMemberAccess = new(nameof(OptionalMemberAccess), precedence: 20);                 // x?.y
    public static readonly OperatorType PostfixIncrement = new(nameof(PostfixIncrement), precedence: 18);                         // x++
    public static readonly OperatorType PostfixDecrement = new(nameof(PostfixDecrement), precedence: 18);                         // x--
    public static readonly OperatorType LogicalNot = new(nameof(LogicalNot), precedence: 17);                                     // ! x
    public static readonly OperatorType BitwiseNot = new(nameof(BitwiseNot), precedence: 17);                                     // ~ x
    public static readonly OperatorType UnaryPlus = new(nameof(UnaryPlus), precedence: 17);                                       // + x
    public static readonly OperatorType UnaryNegation = new(nameof(UnaryNegation), precedence: 17);                               // - x
    public static readonly OperatorType PrefixIncrement = new(nameof(PrefixIncrement), precedence: 17);                           // ++x
    public static readonly OperatorType PrefixDecrement = new(nameof(PrefixDecrement), precedence: 17);                           // --x
    public static readonly OperatorType TypeOf = new(nameof(TypeOf), precedence: 17);                                             // typeof x
    public static readonly OperatorType VoidOf = new(nameof(VoidOf), precedence: 17);                                             // void x
    public static readonly OperatorType As = new(nameof(As), precedence: 17);                                                     // x as y
    public static readonly OperatorType Exponentiation = new(nameof(Exponentiation), precedence: 16);                             // x ** y
    public static readonly OperatorType Multiplication = new(nameof(Multiplication), precedence: 15);                             // x * y
    public static readonly OperatorType Division = new(nameof(Division), precedence: 15);                                         // x / y
    public static readonly OperatorType Remainder = new(nameof(Remainder), precedence: 15);                                       // x % y
    public static readonly OperatorType Addition = new(nameof(Addition), precedence: 14);                                         // x + y
    public static readonly OperatorType Subtraction = new(nameof(Subtraction), precedence: 14);                                   // x - y
    public static readonly OperatorType BitwiseLeftShift = new(nameof(BitwiseLeftShift), precedence: 13);                         // x << y
    public static readonly OperatorType BitwiseRightShift = new(nameof(BitwiseRightShift), precedence: 13);                       // x >> y
    public static readonly OperatorType BitwiseUnsignedRightShift = new(nameof(BitwiseUnsignedRightShift), precedence: 13);       // x >>> y
    public static readonly OperatorType LessThan = new(nameof(LessThan), precedence: 12);                                         // x < y
    public static readonly OperatorType LessThanOrEqual = new(nameof(LessThanOrEqual), precedence: 12);                           // x <= y
    public static readonly OperatorType GreaterThan = new(nameof(GreaterThan), precedence: 12);                                   // x > y
    public static readonly OperatorType GreaterThanOrEqual = new(nameof(GreaterThanOrEqual), precedence: 12);                     // x >= y
    public static readonly OperatorType In = new(nameof(In), precedence: 12);                                                     // x in y
    public static readonly OperatorType InstanceOf = new(nameof(InstanceOf), precedence: 12);                                     // x instanceof y
    public static readonly OperatorType Equality = new(nameof(Equality), precedence: 11);                                         // x == y
    public static readonly OperatorType Inequality = new(nameof(Inequality), precedence: 11);                                     // x != y
    public static readonly OperatorType StrictEquality = new(nameof(StrictEquality), precedence: 11);                             // x === y
    public static readonly OperatorType StrictInequality = new(nameof(StrictInequality), precedence: 11);                         // x !== y
    public static readonly OperatorType BitwiseAnd = new(nameof(BitwiseAnd), precedence: 10);                                     // x & y
    public static readonly OperatorType BitwiseXor = new(nameof(BitwiseXor), precedence: 9);                                      // x ^ y
    public static readonly OperatorType BitwiseOr = new(nameof(BitwiseOr), precedence: 8);                                        // x | y
    public static readonly OperatorType LogicalAnd = new(nameof(LogicalAnd), precedence: 7);                                      // x && y
    public static readonly OperatorType LogicalOr = new(nameof(LogicalOr), precedence: 6);                                        // x || y
    public static readonly OperatorType NullishCoalescing = new(nameof(NullishCoalescing), precedence: 5);                        // x ?? y
    public static readonly OperatorType ConditionalTernary = new(nameof(ConditionalTernary), precedence: 4);                      // x ? y : z
    public static readonly OperatorType Assignment = new(nameof(Assignment), precedence: 3);                                      // x = y
    public static readonly OperatorType AdditionAssignment = new(nameof(AdditionAssignment), precedence: 3);                      // x += y
    public static readonly OperatorType ExponentiationAssignment = new(nameof(ExponentiationAssignment), precedence: 3);          // x **= y
    public static readonly OperatorType SubtractionAssignment = new(nameof(SubtractionAssignment), precedence: 3);                // x -= y
    public static readonly OperatorType MultiplicationAssignment = new(nameof(MultiplicationAssignment), precedence: 3);          // x *= y
    public static readonly OperatorType DivisionAssignment = new(nameof(DivisionAssignment), precedence: 3);                      // x /= y
    public static readonly OperatorType RemainderAssignment = new(nameof(RemainderAssignment), precedence: 3);                    // x %= y
    public static readonly OperatorType BitwiseAndAssignment = new(nameof(BitwiseAndAssignment), precedence: 3);                  // x &= y
    public static readonly OperatorType BitwiseXorAssignment = new(nameof(BitwiseXorAssignment), precedence: 3);                  // x ^= y
    public static readonly OperatorType BitwiseOrAssignment = new(nameof(BitwiseOrAssignment), precedence: 3);                    // x |= y
    public static readonly OperatorType LogicalAndAssignment = new(nameof(LogicalAndAssignment), precedence: 3);                  // x &&= y
    public static readonly OperatorType LogicalOrAssignment = new(nameof(LogicalOrAssignment), precedence: 3);                    // x ||= y
    public static readonly OperatorType LeftShiftAssignment = new(nameof(LeftShiftAssignment), precedence: 3);                    // x <<= y
    public static readonly OperatorType RightShiftAssignment = new(nameof(RightShiftAssignment), precedence: 3);                  // x >>= y
    public static readonly OperatorType UnsignedRightShiftAssignment = new(nameof(UnsignedRightShiftAssignment), precedence: 3);  // x >>>= y
    public static readonly OperatorType LogicalNullishAssignment = new(nameof(LogicalNullishAssignment), precedence: 3);          // x ??= y
    public static readonly OperatorType Comma = new(nameof(Comma), precedence: 1);                                                // x1, x2, x3, ...
}
