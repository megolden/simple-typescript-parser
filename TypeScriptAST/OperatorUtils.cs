using System.Collections.Generic;

namespace TypeScriptAST;

public static class OperatorUtils
{
    private static readonly Dictionary<OperatorType, int> OperatorsPrecedence = new()
    {
        [OperatorType.Grouping]                            = 21,

        [OperatorType.MemberAccess]                        = 20,
        [OperatorType.New]                                 = 20,
        [OperatorType.FunctionCall]                        = 20,
        [OperatorType.OptionalMemberAccess]                = 20,

        [OperatorType.PostfixIncrement]                    = 18,
        [OperatorType.PostfixDecrement]                    = 18,

        [OperatorType.LogicalNot]                          = 17,
        [OperatorType.BitwiseNot]                          = 17,
        [OperatorType.UnaryPlus]                           = 17,
        [OperatorType.UnaryNegation]                       = 17,
        [OperatorType.PrefixIncrement]                     = 17,
        [OperatorType.PrefixDecrement]                     = 17,
        [OperatorType.TypeOf]                              = 17,
        [OperatorType.VoidOf]                              = 17,
        [OperatorType.As]                                  = 17,

        [OperatorType.Exponentiation]                      = 16,

        [OperatorType.Multiplication]                      = 15,
        [OperatorType.Division]                            = 15,
        [OperatorType.Remainder]                           = 15,

        [OperatorType.Addition]                            = 14,
        [OperatorType.Subtraction]                         = 14,

        [OperatorType.BitwiseLeftShift]                    = 13,
        [OperatorType.BitwiseRightShift]                   = 13,
        [OperatorType.BitwiseUnsignedRightShift]           = 13,

        [OperatorType.LessThan]                            = 12,
        [OperatorType.LessThanOrEqual]                     = 12,
        [OperatorType.GreaterThan]                         = 12,
        [OperatorType.GreaterThanOrEqual]                  = 12,
        [OperatorType.In]                                  = 12,
        [OperatorType.InstanceOf]                          = 12,

        [OperatorType.Equality]                            = 11,
        [OperatorType.Inequality]                          = 11,
        [OperatorType.StrictEquality]                      = 11,
        [OperatorType.StrictInequality]                    = 11,

        [OperatorType.BitwiseAnd]                          = 10,

        [OperatorType.BitwiseXor]                          = 9,

        [OperatorType.BitwiseOr]                           = 8,

        [OperatorType.LogicalAnd]                          = 7,

        [OperatorType.LogicalOr]                           = 6,

        [OperatorType.NullishCoalescing]                   = 5,

        [OperatorType.ConditionalTernary]                  = 4,

        [OperatorType.Assignment]                          = 3,
        [OperatorType.AdditionAssignment]                  = 3,
        [OperatorType.ExponentiationAssignment]            = 3,
        [OperatorType.SubtractionAssignment]               = 3,
        [OperatorType.MultiplicationAssignment]            = 3,
        [OperatorType.DivisionAssignment]                  = 3,
        [OperatorType.RemainderAssignment]                 = 3,
        [OperatorType.BitwiseAndAssignment]                = 3,
        [OperatorType.BitwiseXorAssignment]                = 3,
        [OperatorType.BitwiseOrAssignment]                 = 3,
        [OperatorType.LogicalAndAssignment]                = 3,
        [OperatorType.LogicalOrAssignment]                 = 3,
        [OperatorType.LeftShiftAssignment]                 = 3,
        [OperatorType.RightShiftAssignment]                = 3,
        [OperatorType.UnsignedRightShiftAssignment]        = 3,
        [OperatorType.LogicalNullishAssignment]            = 3,

        [OperatorType.Comma]                               = 1
    };

    public static int PrecedenceOf(OperatorType op)
    {
        return OperatorsPrecedence[op];
    }
}
