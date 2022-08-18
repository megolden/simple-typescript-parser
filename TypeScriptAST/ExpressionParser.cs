using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using TypeScriptAST.Declarations;
using TypeScriptAST.Declarations.Types;
using TypeScriptAST.Expressions;
using static TypeScriptAST.OperatorUtils;
using Array = TypeScriptAST.Expressions.Array;
using String = System.String;
using TemplateElement = TypeScriptAST.Expressions.TemplateLiteral.TemplateElement;
using Type = TypeScriptAST.Declarations.Types.Type;

namespace TypeScriptAST;

public class ExpressionParser
{
    private readonly TextParser _parser;
    private readonly TypeResolver _typeResolver;
    private Token _token;

    public ExpressionParser(TextParser parser, ParserOptions options)
    {
        _parser = parser;
        NextToken();
        _typeResolver = new TypeResolver(options.Modules);
    }

    public ExpressionParser(string text, ParserOptions options) : this(new TextParser(text), options)
    {
    }

    public ExpressionParser(string text) : this(new TextParser(text))
    {
    }

    public ExpressionParser(TextParser parser) : this(parser, ParserOptions.GetDefault())
    {
    }

    public Expression Parse()
    {
        return ReadExpression(priorOperator: null);
    }

    private Expression? TryReadExpression(OperatorType? priorOperator)
    {
        Expression? expression = null;
        while (_token.Type is not TokenType.End)
        {
            if (_token.Type
                is TokenType.CloseBracket
                or TokenType.CloseParen)
            {
                if (expression is null)
                    throw new InvalidTokenException(_token);
                return expression;
            }

            if (_token.Type
                is TokenType.End
                or TokenType.Colon
                or TokenType.Comma
                or TokenType.TemplateMiddle
                or TokenType.TemplateTail)
                return expression;

            if (ResolveOperatorType(hasLeftOperand: expression is not null) is { } currentOperator)
            {
                if (HasHigherPrecedence(currentOperator, priorOperator))
                    return expression;
            }
            else
            {
                if (expression is not null)
                    throw new InvalidTokenException(_token);
            }

            if (ResolveExpression(expression) is { } result)
            {
                expression = result;
            }
            else
            {
                throw new InvalidTokenException(_token);
            }
        }

        return expression;
    }

    private OperatorType? ResolveOperatorType(bool hasLeftOperand)
    {
        return _token.Type switch
        {
            TokenType.Minus =>
                hasLeftOperand ? OperatorType.Subtraction : OperatorType.UnaryNegation,
            TokenType.Plus =>
                hasLeftOperand ? OperatorType.Addition : OperatorType.UnaryPlus,
            TokenType.Asterisk =>
                OperatorType.Multiplication,
            TokenType.Slash =>
                OperatorType.Division,
            TokenType.Percent =>
                OperatorType.Remainder,
            TokenType.DoubleAsterisk =>
                OperatorType.Exponentiation,
            TokenType.Exclamation =>
                OperatorType.LogicalNot,
            TokenType.DoubleAmpersand =>
                OperatorType.LogicalAnd,
            TokenType.DoubleBar =>
                OperatorType.LogicalOr,
            TokenType.Tilde =>
                OperatorType.BitwiseNot,
            TokenType.Carat =>
                OperatorType.BitwiseXor,
            TokenType.Bar =>
                OperatorType.BitwiseOr,
            TokenType.Ampersand =>
                OperatorType.BitwiseAnd,
            TokenType.DoubleLessThan =>
                OperatorType.BitwiseLeftShift,
            TokenType.DoubleGreaterThan =>
                OperatorType.BitwiseRightShift,
            TokenType.TripleGreaterThan =>
                OperatorType.BitwiseUnsignedRightShift,
            TokenType.LessThan =>
                OperatorType.LessThan,
            TokenType.LessThanEqual =>
                OperatorType.LessThanOrEqual,
            TokenType.GreaterThan =>
                OperatorType.GreaterThan,
            TokenType.GreaterThanEqual =>
                OperatorType.GreaterThanOrEqual,
            TokenType.DoubleEqual =>
                OperatorType.Equality,
            TokenType.ExclamationEqual =>
                OperatorType.Inequality,
            TokenType.TripleEqual =>
                OperatorType.StrictEquality,
            TokenType.ExclamationDoubleEqual =>
                OperatorType.StrictInequality,
            TokenType.DoubleQuestion =>
                OperatorType.NullishCoalescing,
            TokenType.Question or TokenType.Colon =>
                OperatorType.ConditionalTernary,
            TokenType.OpenParen or TokenType.OpenBracket =>
                OperatorType.Grouping,
            TokenType.Dot =>
                OperatorType.MemberAccess,
            TokenType.QuestionDot =>
                OperatorType.OptionalMemberAccess,
            TokenType.Comma =>
                OperatorType.Comma,
            TokenType.DoublePlus =>
                hasLeftOperand ? OperatorType.PostfixIncrement : OperatorType.PrefixIncrement,
            TokenType.DoubleMinus =>
                hasLeftOperand ? OperatorType.PostfixDecrement : OperatorType.PrefixDecrement,
            TokenType.Identifier when _token.Text == "as" =>
                OperatorType.As,
            TokenType.Identifier when _token.Text == "instanceof" =>
                OperatorType.InstanceOf,
            TokenType.Identifier when _token.Text == "in" =>
                OperatorType.In,
            TokenType.Identifier when _token.Text == "void" =>
                OperatorType.VoidOf,
            TokenType.Identifier when _token.Text == "typeof" =>
                OperatorType.TypeOf,
            _ => null
        };
    }

    private bool HasHigherPrecedence(OperatorType currentOperator, OperatorType? priorOperatorType)
    {
        return priorOperatorType is { } priorOperator
               and not OperatorType.Grouping
               && PrecedenceOf(currentOperator) <= PrecedenceOf(priorOperator);
    }

    private Expression? ResolveExpression(Expression? left)
    {
        return ResolveAdditionOrUnaryPlus(left)
            ?? ResolveSubtractionOrNegation(left)
            ?? ResolveMultiplication(left)
            ?? ResolveDivision(left)
            ?? ResolveRemainder(left)
            ?? ResolveExponentiation(left)
            ?? ResolveLogicalOperator(left)
            ?? ResolveBitwiseOperator(left)
            ?? ResolveComparisonOperator(left)
            ?? ResolveNullishCoalescing(left)
            ?? ResolveConditional(left)
            ?? ResolveBuiltInOperator(left)
            ?? ResolveFunctionCall(left)
            ?? ResolveNew()
            ?? ResolveMemberAccess(left)
            ?? ResolveMemberIndex(left)
            ?? ResolveIncDecOperator(left)
            ?? ResolveNumericLiteral()
            ?? ResolveStringLiteral()
            ?? ResolveRegExpLiteral()
            ?? ResolveNoSubstitutionTemplateLiteral()
            ?? ResolveTemplateLiteral()
            ?? ResolveKeywordsLiteral()
            ?? ResolveGroup()
            ?? ResolveIdentifier()
            ?? ResolveArray(left is not null);
    }

    private Literal? ResolveKeywordsLiteral()
    {
        if (_token.Type is TokenType.NullLiteral)
        {
            NextToken();
            return Expression.Literal(null, Type.Any);
        }
        if (_token.Type is TokenType.Identifier && _token.Text == "undefined")
        {
            NextToken();
            return Expression.Literal(Undefined.Value, Type.Any);
        }
        if (_token.Type is TokenType.Identifier && _token.Text == "NaN")
        {
            NextToken();
            return Expression.Literal(Double.NaN, Type.Number);
        }
        if (_token.Type is TokenType.Identifier && _token.Text == "Infinity")
        {
            NextToken();
            return Expression.Literal(Double.PositiveInfinity, Type.Number);
        }
        if (_token.Type is TokenType.BooleanLiteral)
        {
            var value = _token.Text == "true";
            NextToken();
            return Expression.Literal(value, Type.Boolean);
        }

        return null;
    }

    private Literal? ResolveNumericLiteral()
    {
        if (_token.Type is not TokenType.NumericLiteral)
            return null;

        const char LiteralSeparator = '_';
        const char DecimalSeparator = '.';
        var isBigInt = _token.Text.EndsWith('n');
        var isBin = _token.Text.StartsWith("0b", ignoreCase: true, CultureInfo.InvariantCulture);
        var isOct = _token.Text.StartsWith("0o", ignoreCase: true, CultureInfo.InvariantCulture) ||
                    (_token.Text.StartsWith('0') && _token.Text.Length > 1 && _token.Text[1] is >= '0' and <= '7');
        var isHex = _token.Text.StartsWith("0x", ignoreCase: true, CultureInfo.InvariantCulture);
        var isReal = _token.Text.Contains(DecimalSeparator);
        var hasExp = _token.Text.Contains('e', StringComparison.OrdinalIgnoreCase);
        var text = _token.Text
            .Replace(LiteralSeparator.ToString(), String.Empty)
            .TrimEnd('n');
        object ApplyBigInt(long value) => isBigInt ? (object)new BigInteger(value) : value;

        var value = text switch
        {
            _ when isHex =>
                ApplyBigInt(Convert.ToInt64(text.Substring(2), 16)),
            _ when isBin =>
                ApplyBigInt(Convert.ToInt64(text.Substring(2), 2)),
            _ when isOct =>
                ApplyBigInt(Convert.ToInt64(text[1] is 'o' or 'O' ? text.Substring(2) : text.Substring(1), 8)),
            _ when hasExp || isReal =>
                Convert.ToDouble(text),
            _ =>
                ApplyBigInt(Convert.ToInt64(text, 10)),
        };

        NextToken();
        var type = isBigInt ? Type.BigInt : Type.Number;
        return Expression.Literal(value, type);
    }

    private Literal? ResolveStringLiteral()
    {
        if (_token.Type is not TokenType.StringLiteral)
            return null;

        const char EscapeCharacter = '\\';

        var buffer = new StringBuilder();
        var lastIndex = _token.Text.Length - 2;
        for (var index = 1; index <= lastIndex;)
        {
            var ch = _token.Text[index];

            if (ch is EscapeCharacter)
            {
                index++;
                var escaped = GetStringEscaped(_token, index, out var length);
                if (!IsLineTerminator(escaped[0])) buffer.Append(escaped);
                index += length;
            }
            else
            {
                buffer.Append(ch);
                index++;
            }
        }

        NextToken();
        return Expression.Literal(buffer.ToString(), Type.String);
    }

    private RegularExpression? ResolveRegExpLiteral()
    {
        if (_token.Type is not TokenType.RegExpLiteral)
            return null;

        const char EscapeCharacter = '\\';

        var buffer = new StringBuilder();
        var lastIndex = _token.Text.Length - 1;
        var index = 1;
        while (index <= lastIndex)
        {
            var ch = _token.Text[index];

            if (ch is EscapeCharacter)
            {
                var nextChar = index < lastIndex ? _token.Text[index + 1] : '\0';
                if (nextChar is '/')
                {
                    buffer.Append('/');
                    index += 2;
                    continue;
                }
                else
                {
                    buffer.Append(EscapeCharacter);
                    index++;
                    continue;
                }
            }

            if (ch is '/')
            {
                index++;
                break;
            }

            buffer.Append(ch);
            index++;
        }
        var flags = index <= lastIndex ? _token.Text.Substring(index) : String.Empty;

        NextToken();
        return Expression.RegularExpression(buffer.ToString(), flags);
    }

    private TemplateLiteral? ResolveNoSubstitutionTemplateLiteral()
    {
        if (_token.Type is not TokenType.TemplateLiteral)
            return null;

        const char EscapeCharacter = '\\';

        var buffer = new StringBuilder();
        var lastIndex = _token.Text.Length - 2;
        for (var index = 1; index <= lastIndex;)
        {
            var ch = _token.Text[index];

            if (ch is EscapeCharacter)
            {
                index++;
                var escaped = GetStringEscaped(_token, index, out var length);
                buffer.Append(escaped);
                index += length;
            }
            else
            {
                buffer.Append(ch);
                index++;
            }
        }

        NextToken();
        return Expression.TemplateLiteral(new TemplateElement[] { buffer.ToString() });
    }

    private TemplateLiteral? ResolveTemplateLiteral()
    {
        if (_token.Type is not TokenType.TemplateHead)
            return null;

        var head = ResolveTemplateValueElement();
        var elements = new List<TemplateElement> { head };
        while (_token.Type is not TokenType.End)
        {
            if (_token.Type is TokenType.TemplateMiddle)
            {
                elements.Add(ResolveTemplateValueElement());
            }
            else if (_token.Type is TokenType.TemplateTail)
            {
                elements.Add(ResolveTemplateValueElement());
                break;
            }
            else
            {
                elements.Add(ReadExpression(priorOperator: null));
            }
        }
        return Expression.TemplateLiteral(elements);
    }

    private TemplateElement ResolveTemplateValueElement()
    {
        const char EscapeCharacter = '\\';

        var buffer = new StringBuilder();
        var lastIndex = _token.Text.Length - (_token.Type is TokenType.TemplateTail ? 2 : 3);
        for (var index = 1; index <= lastIndex;)
        {
            var ch = _token.Text[index];

            if (ch is EscapeCharacter)
            {
                index++;
                var escaped = GetStringEscaped(_token, index, out var length);
                buffer.Append(escaped);
                index += length;
            }
            else
            {
                buffer.Append(ch);
                index++;
            }
        }

        NextToken();
        return buffer.ToString();
    }

    private string GetStringEscaped(Token token, int fromIndex, out int length)
    {
        bool IsDigit(char ch) => ch is >= '0' and <= '9';
        bool IsHexDigit(char ch) => ch is (>= '0' and <= '9') or (>= 'a' and <= 'f') or (>= 'A' and <= 'F');
        bool IsOctalDigit(char ch) => ch is >= '0' and <= '7';
        bool GetEscapeChar(char ch, out char eChar)
        {
            switch (ch)
            {
                case 'b':
                    eChar = '\b';
                    break;
                case 't':
                    eChar = '\t';
                    break;
                case 'n':
                    eChar = '\n';
                    break;
                case 'v':
                    eChar = '\v';
                    break;
                case 'f':
                    eChar = '\f';
                    break;
                case 'r':
                    eChar = '\r';
                    break;
                default:
                    eChar = '\0';
                    return false;
            }
            return true;
        }
        string ReadHexInteger(string str, int from = 0, int maxLen = 0)
        {
            if (maxLen == 0) maxLen = str.Length - from;

            var index = from;
            for (; index < str.Length && maxLen > 0 && IsHexDigit(str[index]); index++, maxLen--) { }

            return str.Substring(from, index - from);
        }
        char PeekChar(int index) => index < token.Text.Length ? token.Text[index] : '\0';

        var index = fromIndex;
        var ch = PeekChar(index);
        var nextChar = PeekChar(index + 1);

        void NextChar()
        {
            if (index < token.Text.Length)
            {
                index++;
                ch = PeekChar(index);
                nextChar = PeekChar(index + 1);
            }
            else
            {
                ch = '\0';
                nextChar = '\0';
            }
        }

        // line terminator
        if (ch is '\u000D' && nextChar is '\u000A')
        {
            length = 2;
            return ch.ToString() + nextChar;
        }
        if (IsLineTerminator(ch))
        {
            length = 1;
            return ch.ToString();
        }

        // escape characters
        if (GetEscapeChar(ch, out var eChar))
        {
            length = 1;
            return eChar.ToString();
        }

        // hex code point
        if (ch is 'x')
        {
            NextChar(); // x
            if (ReadHexInteger(token.Text, index, maxLen: 2) is { Length: 2 } digits)
            {
                length = 3;
                var code = Convert.ToInt32(digits, 16);
                return Char.ConvertFromUtf32(code);
            }
            else
            {
                throw new InvalidTokenException(token);
            }
        }
        if (ch is 'u')
        {
            NextChar(); // u
            if (ch is '{')
            {
                NextChar(); // {
                if (ReadHexInteger(token.Text, index) is { Length: > 0 } digits)
                {
                    if (PeekChar(index + digits.Length) is not '}')
                        throw new InvalidTokenException(token);

                    length = 3 + digits.Length;
                    var code = Convert.ToInt32(digits, 16);
                    return Char.ConvertFromUtf32(code);
                }
                else
                {
                    throw new InvalidTokenException(token);
                }
            }
            else
            {
                if (ReadHexInteger(token.Text, index, maxLen: 4) is { Length: 4 } digits)
                {
                    length = 5;
                    var code = Convert.ToInt32(digits, 16);
                    return Char.ConvertFromUtf32(code);
                }
                else
                {
                    throw new InvalidTokenException(token);
                }
            }
        }

        // octal code point
        if (IsOctalDigit(ch))
        {
            if (!IsOctalDigit(nextChar))
            {
                length = 1;
                var code = Convert.ToInt32(ch.ToString(), 8);
                return Char.ConvertFromUtf32(code);
            }
            if (ch >= '4' && IsOctalDigit(nextChar))
            {
                length = 2;
                var code = Convert.ToInt32(ch.ToString() + nextChar, 8);
                return Char.ConvertFromUtf32(code);
            }
            if (IsOctalDigit(nextChar))
            {
                if (PeekChar(index + 2) is var next2Char && IsOctalDigit(next2Char))
                {
                    length = 3;
                    var digits = ch.ToString() + nextChar + next2Char;
                    var code = Convert.ToInt32(digits, 8);
                    return Char.ConvertFromUtf32(code);
                }
                else
                {
                    length = 2;
                    var digits = ch.ToString() + nextChar;
                    var code = Convert.ToInt32(digits, 8);
                    return Char.ConvertFromUtf32(code);
                }
            }
        }

        // null char
        if (ch is '0' && !IsDigit(nextChar))
        {
            length = 1;
            return "\0";
        }

        length = 1;
        return ch.ToString();
    }

    private Expression? ResolveIdentifier(bool onlyTypes = false)
    {
        if (_token.Type is not TokenType.Identifier)
            return null;

        var name = _token.Text;
        do
        {
            if (ResolveName(name) is { } expression)
                return expression;

            if (_typeResolver.NamespaceExists(name))
            {
                NextToken();
                if (_token.Type is TokenType.Dot)
                {
                    NextToken();
                    if (_token.Type is TokenType.Identifier)
                    {
                        name += "." + _token.Text;
                        continue;
                    }
                }
            }
            return null;
        } while (_token.Type is not TokenType.End);

        return null;

        Expression? ResolveName(string name)
        {
            if (!onlyTypes && _typeResolver.ResolveMember(name) is { } member)
            {
                NextToken();
                return ResolveConstructorOrType(member);
            }

            if (_typeResolver.ResolveType(name) is { } type)
            {
                NextToken();
                return Expression.Literal(type, type);
            }

            return null;
        }

        Expression ResolveConstructorOrType(IMemberInfo member)
        {
            if (member is FunctionDefinition { IsConstructor: true } ctor &&
                !TypeResolver.BuiltInTypes.Contains(ctor.DeclaringType))
                return Expression.Literal(ctor.DeclaringType, ctor.DeclaringType);
            return Expression.Identifier(member);
        }
    }

    private Type? ResolveType()
    {
        return ResolveIdentifier(onlyTypes: true) is Literal { Value: Type type } ? type : null;
    }

    private Array? ResolveArray(bool hasLeftOperand)
    {
        if (_token.Type is not TokenType.OpenBracket || hasLeftOperand)
            return null;

        NextToken(); // [
        var items = new List<Expression>();
        while (_token.Type is not TokenType.CloseBracket and not TokenType.End)
        {
            items.Add(ReadExpression(priorOperator: null));
            if (_token.Type is TokenType.Comma)
                NextToken();
        }

        if (_token.Type is not TokenType.CloseBracket)
            throw new InvalidTokenException(_token);

        NextToken(); // ]
        return Expression.Array(items);
    }

    private Operator? ResolveIncDecOperator(Expression? left)
    {
        if (_token.Type is not TokenType.DoublePlus and not TokenType.DoubleMinus)
            return null;

        var isIncrement = _token.Type is TokenType.DoublePlus;

        if (left is not null)
        {
            NextToken(); // ++ or --
            return isIncrement
                ? Expression.PostfixIncrement(left)
                : Expression.PostfixDecrement(left);
        }

        var opType = isIncrement ?
            OperatorType.PrefixIncrement :
            OperatorType.PrefixDecrement;

        var operand = ReadNextExpression(opType);

        return isIncrement
            ? Expression.PrefixIncrement(operand)
            : Expression.PrefixDecrement(operand);
    }

    private Expression? ResolveAdditionOrUnaryPlus(Expression? left)
    {
        if (_token.Type is not TokenType.Plus)
            return null;

        return left is null
            ? Expression.UnaryPlus(ReadNextExpression(OperatorType.UnaryPlus))
            : Expression.Add(left, ReadNextExpression(OperatorType.Addition));
    }

    private Expression? ResolveSubtractionOrNegation(Expression? left)
    {
        if (_token.Type is not TokenType.Minus)
            return null;

        return left is null
            ? Expression.Negate(ReadNextExpression(OperatorType.UnaryNegation))
            : Expression.Subtract(left, ReadNextExpression(OperatorType.Subtraction));
    }

    private Multiply? ResolveMultiplication(Expression? left)
    {
        if (_token.Type is not TokenType.Asterisk)
            return null;

        if (left is null)
            throw new InvalidTokenException(_token);

        return Expression.Multiply(left, ReadNextExpression(OperatorType.Multiplication));
    }

    private Divide? ResolveDivision(Expression? left)
    {
        if (_token.Type is not TokenType.Slash)
            return null;

        if (left is null)
            throw new InvalidTokenException(_token);

        return Expression.Divide(left, ReadNextExpression(OperatorType.Division));
    }

    private Remainder? ResolveRemainder(Expression? left)
    {
        if (_token.Type is not TokenType.Percent)
            return null;

        if (left is null)
            throw new InvalidTokenException(_token);

        return Expression.Remainder(left, ReadNextExpression(OperatorType.Remainder));
    }

    private Exponent? ResolveExponentiation(Expression? left)
    {
        if (_token.Type is not TokenType.DoubleAsterisk)
            return null;

        if (left is null)
            throw new InvalidTokenException(_token);

        return Expression.Exponentiation(left, ReadNextExpression(OperatorType.Exponentiation));
    }

    private Operator? ResolveLogicalOperator(Expression? left)
    {
        switch (_token.Type)
        {
            case TokenType.Exclamation:
                return Expression.LogicalNot(ReadNextExpression(OperatorType.LogicalNot));

            case TokenType.DoubleAmpersand:
                if (left is null)
                    throw new InvalidTokenException(_token);
                return Expression.LogicalAnd(left, ReadNextExpression(OperatorType.LogicalAnd));

            case TokenType.DoubleBar:
                if (left is null)
                    throw new InvalidTokenException(_token);
                return Expression.LogicalOr(left, ReadNextExpression(OperatorType.LogicalOr));

            default:
                return null;
        }
    }

    private Operator? ResolveBitwiseOperator(Expression? left)
    {
        switch (_token.Type)
        {
            case TokenType.Tilde:
                return Expression.BitwiseNot(ReadNextExpression(OperatorType.BitwiseNot));

            case TokenType.Ampersand:
                if (left is null)
                    throw new InvalidTokenException(_token);
                return Expression.BitwiseAnd(left, ReadNextExpression(OperatorType.BitwiseAnd));

            case TokenType.Bar:
                if (left is null)
                    throw new InvalidTokenException(_token);
                return Expression.BitwiseOr(left, ReadNextExpression(OperatorType.BitwiseOr));

            case TokenType.Carat:
                if (left is null)
                    throw new InvalidTokenException(_token);
                return Expression.BitwiseXor(left, ReadNextExpression(OperatorType.BitwiseXor));

            case TokenType.DoubleLessThan:
                if (left is null)
                    throw new InvalidTokenException(_token);
                return Expression.BitwiseLeftShift(left, ReadNextExpression(OperatorType.BitwiseLeftShift));

            case TokenType.DoubleGreaterThan:
                if (left is null)
                    throw new InvalidTokenException(_token);
                return Expression.BitwiseRightShift(left, ReadNextExpression(OperatorType.BitwiseRightShift));

            case TokenType.TripleGreaterThan:
                if (left is null)
                    throw new InvalidTokenException(_token);
                return Expression.BitwiseUnsignedRightShift(
                    left,
                    ReadNextExpression(OperatorType.BitwiseUnsignedRightShift));

            default:
                return null;
        }
    }

    private Operator? ResolveComparisonOperator(Expression? left)
    {
        switch (_token.Type)
        {
            case TokenType.LessThan:
                if (left is null)
                    throw new InvalidTokenException(_token);
                return Expression.LessThan(left, ReadNextExpression(OperatorType.LessThan));

            case TokenType.LessThanEqual:
                if (left is null)
                    throw new InvalidTokenException(_token);
                return Expression.LessThanOrEqual(left, ReadNextExpression(OperatorType.LessThanOrEqual));

            case TokenType.GreaterThan:
                if (left is null)
                    throw new InvalidTokenException(_token);
                return Expression.GreaterThan(left, ReadNextExpression(OperatorType.GreaterThan));

            case TokenType.GreaterThanEqual:
                if (left is null)
                    throw new InvalidTokenException(_token);
                return Expression.GreaterThanOrEqual(left, ReadNextExpression(OperatorType.GreaterThanOrEqual));

            case TokenType.DoubleEqual:
                if (left is null)
                    throw new InvalidTokenException(_token);
                return Expression.Equality(left, ReadNextExpression(OperatorType.Equality));

            case TokenType.ExclamationEqual:
                if (left is null)
                    throw new InvalidTokenException(_token);
                return Expression.Inequality(left, ReadNextExpression(OperatorType.Inequality));

            case TokenType.TripleEqual:
                if (left is null)
                    throw new InvalidTokenException(_token);
                return Expression.StrictEquality(left, ReadNextExpression(OperatorType.StrictEquality));

            case TokenType.ExclamationDoubleEqual:
                if (left is null)
                    throw new InvalidTokenException(_token);
                return Expression.StrictInequality(left, ReadNextExpression(OperatorType.StrictInequality));

            default:
                return null;
        }
    }

    private NullishCoalescing? ResolveNullishCoalescing(Expression? left)
    {
        if (_token.Type is not TokenType.DoubleQuestion)
            return null;

        if (left is null)
            throw new InvalidTokenException(_token);

        return Expression.NullishCoalescing(left, ReadNextExpression(OperatorType.NullishCoalescing));
    }

    private Conditional? ResolveConditional(Expression? left)
    {
        if (_token.Type is not TokenType.Question)
            return null;

        if (left is null)
            throw new InvalidTokenException(_token);

        var condition = left;
        NextToken(); // ?
        var trueExpression = ReadExpression(OperatorType.ConditionalTernary);
        NextToken(); // :
        var falseExpression = ReadExpression(OperatorType.ConditionalTernary);
        return Expression.Conditional(condition, trueExpression, falseExpression);
    }

    private Expression? ResolveGroup()
    {
        if (_token.Type is not TokenType.OpenParen)
            return null;

        NextToken(); // (
        var expression = ReadExpression(OperatorType.Grouping);

        if (_token.Type is not TokenType.CloseParen)
            throw new InvalidTokenException(_token);

        NextToken(); // )
        return expression;
    }

    private Operator? ResolveMemberAccess(Expression? left)
    {
        if (_token.Type is not TokenType.Dot and not TokenType.QuestionDot)
            return null;

        if (left is null)
            throw new InvalidTokenException(_token);

        var isOptional = _token.Type is TokenType.QuestionDot;

        NextToken(); // . or ?.

        if (_token.Type is TokenType.Identifier)
        {
            var member = TypeResolver.GetMember(left.Type, _token.Text)
                         ?? throw new InvalidTokenException(_token);

            NextToken(); // identifier

            return Expression.MemberAccess(left, member, isOptional);
        }
        else if (!isOptional)
        {
            throw new InvalidTokenException(_token);
        }
        else if (_token.Type is TokenType.OpenBracket)
        {
            return ResolveMemberIndex(left, isOptional);
        }
        else if (_token.Type is TokenType.OpenParen)
        {
            return ResolveFunctionCall(left, isOptional);
        }
        else
        {
            throw new InvalidTokenException(_token);
        }
    }

    private MemberIndex? ResolveMemberIndex(Expression? left, bool isOptional = false)
    {
        if (_token.Type is not TokenType.OpenBracket || left is null)
            return null;

        NextToken(); // [
        var member = ReadExpression(OperatorType.Grouping);

        if (_token.Type is not TokenType.CloseBracket)
            throw new InvalidTokenException(_token);

        NextToken(); // ]
        return Expression.MemberIndex(left, member, isOptional);
    }

    private FunctionCall? ResolveFunctionCall(Expression? left, bool isOptional = false)
    {
        if (_token.Type is not TokenType.OpenParen || left is null)
            return null;

        var args = ReadArguments(OperatorType.FunctionCall);

        if (left is Identifier { Subject: FunctionDeclaration rawFunc })
        {
            var function =
                FindMatchingFunction(rawFunc, args)
                ?? throw new InvalidOperationException($"invalid function signature: {rawFunc}");
            var expression = Expression.Identifier(function);
            return Expression.FunctionCall(expression, args, false);
        }
        else if (left is MemberAccess { Member: FunctionDeclaration or FunctionDefinition } memberAccess)
        {
            var function =
                FindMatchingFunction(memberAccess.Member, args)
                ?? throw new InvalidOperationException($"invalid function signature: {memberAccess.Member}");
            var expression = Expression.MemberAccess(memberAccess.Instance, function, memberAccess.IsOptional);
            return Expression.FunctionCall(expression, args, isOptional);
        }
        else if (left is Literal { Value: Type type } && !TypeResolver.BuiltInTypes.Contains(type))
        {
            throw new InvalidOperationException($"new keyword is required: {type}");
        }
        else
        {
            return Expression.FunctionCall(left, args, isOptional);
        }
    }

    private Expression[] ReadArguments(OperatorType? priorOperator)
    {
        if (_token.Type is not TokenType.OpenParen)
            throw new InvalidTokenException(_token);

        var args = new List<Expression>();
        NextToken(); // (
        while (_token.Type is not TokenType.CloseParen and not TokenType.End)
        {
            args.Add(ReadExpression(priorOperator));
            if (_token.Type is TokenType.Comma)
                NextToken();
        }

        if (_token.Type is not TokenType.CloseParen)
            throw new InvalidTokenException(_token);

        NextToken(); // )

        return args.ToArray();
    }

    private IMemberInfo? FindMatchingFunction(IMemberInfo function, IReadOnlyList<Expression> args)
    {
        if (function is FunctionDefinition { IsConstructor: true } ctor)
            return ctor.DeclaringType.Constructors.FirstOrDefault(_ => IsMatchArguments(_, args));

        return _typeResolver.ResolveMembers(function.FullName)
                            .FirstOrDefault(func => IsMatchArguments(func, args));
    }

    private bool IsMatchArguments(IMemberInfo function, IReadOnlyList<Expression> args, bool throwIfNotMatch = false)
    {
        var parameters =
            function is FunctionDefinition funcDef ? funcDef.Parameters :
            function is FunctionDeclaration funcDec ? funcDec.Parameters :
            throw new ArgumentException($"member is not function: {function.Name}", nameof(function));

        bool CheckNumberOfArguments()
        {
            var min = parameters.Count == 0 || parameters.FirstOrDefault() is { IsRest: true } or { IsOptional : true }
                ? 0
                : parameters.Count(_ => !_.IsRest && !_.IsOptional);
            var max = parameters.Any(_ => _.IsRest) ? Int32.MaxValue : parameters.Count;

            return args.Count >= min && args.Count <= max;
        }

        bool CheckTypeOfArguments()
        {
            Type ResolveParameterType(int index)
            {
                var restParamIndex = parameters.FindIndex(_ => _.IsRest);
                return restParamIndex >= 0 && index >= restParamIndex
                    ? parameters[restParamIndex].Type.UnderlyingType
                    : parameters[index].Type;
            }

            return args
                .Select((arg, index) => (Index: index, Type: arg.Type)).ToList()
                .All(arg => ResolveParameterType(arg.Index).IsAssignableFrom(arg.Type));
        }

        if (!CheckNumberOfArguments())
        {
            if (throwIfNotMatch)
                throw new ArgumentException($"wrong number of arguments: {function.FullName}");
            return false;
        }

        if (!CheckTypeOfArguments())
        {
            if (throwIfNotMatch)
                throw new ArgumentException($"wrong type of argument(s): {function.FullName}");
            return false;
        }

        return true;
    }

    private Operator? ResolveBuiltInOperator(Expression? left)
    {
        if (_token.Type is not TokenType.Identifier)
            return null;

        switch (_token.Text)
        {
            case "instanceof":
                if (left is null)
                    throw new InvalidTokenException(_token);

                NextToken(); // instanceof

                var iHasParen = _token.Type is TokenType.OpenParen;
                if (iHasParen) NextToken(); // (
                var instanceOfTargetType =
                    ResolveType()
                    ?? throw new InvalidTokenException(_token);

                if (iHasParen)
                {
                    if (_token.Type is TokenType.CloseParen)
                        NextToken(); // )
                    else
                        throw new InvalidTokenException(_token);
                }

                return Expression.InstanceOf(left, instanceOfTargetType);

            case "as":
                if (left is null)
                    throw new InvalidTokenException(_token);

                NextToken(); // as

                var aHasParen = _token.Type is TokenType.OpenParen;
                if (aHasParen) NextToken(); // (

                var asTargetType =
                    ResolveType()
                    ?? throw new InvalidTokenException(_token);

                if (aHasParen)
                {
                    if (_token.Type is TokenType.CloseParen)
                        NextToken(); // )
                    else
                        throw new InvalidTokenException(_token);
                }

                return Expression.As(left, asTargetType);

            case "in":
                if (left is null)
                    throw new InvalidTokenException(_token);

                NextToken(); // in
                var inExpression = ResolveType() is { } inType ?
                    Expression.Literal(inType, inType) :
                    ReadExpression(OperatorType.In);
                return Expression.In(left, inExpression);

            case "typeof":
                NextToken(); // typeof

                var tHasParen = _token.Type is TokenType.OpenParen;
                if (tHasParen) NextToken(); // (
                var typeOfExpression = ResolveType() is { } typeOfType ?
                    Expression.Literal(typeOfType, typeOfType) :
                    ReadExpression(OperatorType.TypeOf);

                if (tHasParen)
                {
                    if (_token.Type is TokenType.CloseParen)
                        NextToken(); // )
                    else
                        throw new InvalidTokenException(_token);
                }

                return Expression.TypeOf(typeOfExpression);

            case "void":
                NextToken(); // typeof

                var vHasParen = _token.Type is TokenType.OpenParen;
                if (vHasParen) NextToken(); // (
                var voidOfExpression = ResolveType() is { } voidOfType ?
                    Expression.Literal(voidOfType, voidOfType) :
                    ReadExpression(OperatorType.VoidOf);

                if (vHasParen)
                {
                    if (_token.Type is TokenType.CloseParen)
                        NextToken(); // )
                    else
                        throw new InvalidTokenException(_token);
                }

                return Expression.VoidOf(voidOfExpression);

            default:
                return null;
        }
    }

    private New? ResolveNew()
    {
        if (_token.Type is not TokenType.Identifier || _token.Text != "new")
            return null;

        NextToken(); // new

        if (ResolveType() is { } type)
        {
            var args = ReadArguments(OperatorType.New);

            var constructor =
                type.Constructors.FirstOrDefault() is { } rawCtor &&
                FindMatchingFunction(rawCtor, args) is FunctionDefinition ctor
                ? ctor : null;

            if (constructor is null)
                throw new InvalidOperationException($"invalid constructor signature: {type}");

            return Expression.New(constructor, args);
        }
        else
        {
            throw new InvalidTokenException(_token);
        }
    }

    private void NextToken(bool skipComments = true)
    {
        _token = _parser.ReadToken();
        if (skipComments)
        {
            while (_token.Type is TokenType.Comment)
                _token = _parser.ReadToken();
        }
    }

    private Expression ReadExpression(OperatorType? priorOperator)
    {
        return TryReadExpression(priorOperator) ??
               throw new InvalidTokenException(_token);
    }

    private Expression ReadNextExpression(OperatorType? priorOperator, bool skipComments = true)
    {
        NextToken(skipComments);
        return TryReadExpression(priorOperator) ??
               throw new InvalidTokenException(_token);
    }

    private bool IsLineTerminator(char character)
    {
        return character is '\u000A' or '\u2028' or '\u2029' or '\u000D';
    }
}
