namespace TypeScriptAST;

public enum OperatorType
{
    Addition,                             // x + y
    Subtraction,                          // x - y
    Multiplication,                       // x * y
    Division,                             // x / y
    UnaryNegation,                        // - x
    UnaryPlus,                            // + x
    Remainder,                            // x % y
    Exponentiation,                       // x ** y
    LogicalNot,                           // ! x
    LogicalAnd,                           // x && y
    LogicalOr,                            // x || y
    BitwiseNot,                           // ~ x
    BitwiseAnd,                           // x & y
    BitwiseOr,                            // x | y
    BitwiseXor,                           // x ^ y
    BitwiseLeftShift,                     // x << y
    BitwiseRightShift,                    // x >> y
    BitwiseUnsignedRightShift,            // x >>> y
    LessThan,                             // x < y
    LessThanOrEqual,                      // x <= y
    GreaterThan,                          // x > y
    GreaterThanOrEqual,                   // x >= y
    Equality,                             // x == y
    Inequality,                           // x != y
    StrictEquality,                       // x === y
    StrictInequality,                     // x !== y
    NullishCoalescing,                    // x ?? y
    ConditionalTernary,                   // x ? y : z
    Grouping,                             // (x)
    MemberAccess,                         // x.y
    OptionalMemberAccess,                 // x?.y
    New,                                  // new x(...)
    FunctionCall,                         // x(...)
    Comma,                                // x1, x2, x3, ...
    Assignment,                           // x = y
    MultiplicationAssignment,             // x *= y
    DivisionAssignment,                   // x /= y
    RemainderAssignment,                  // x %= y
    AdditionAssignment,                   // x += y
    SubtractionAssignment,                // x -= y
    BitwiseAndAssignment,                 // x &= y
    BitwiseXorAssignment,                 // x ^= y
    BitwiseOrAssignment,                  // x |= y
    LogicalAndAssignment,                 // x &&= y
    LogicalOrAssignment,                  // x ||= y
    LogicalNullishAssignment,             // x ??= y
    ExponentiationAssignment,             // x **= y
    LeftShiftAssignment,                  // x <<= y
    RightShiftAssignment,                 // x >>= y
    UnsignedRightShiftAssignment,         // x >>>= y
    PostfixIncrement,                     // x++
    PostfixDecrement,                     // x--
    PrefixIncrement,                      // ++x
    PrefixDecrement,                      // --x
    InstanceOf,                           // x instanceof y
    In,                                   // x in y
    As,                                   // x as y
    VoidOf,                               // void x
    TypeOf                                // typeof x
}
