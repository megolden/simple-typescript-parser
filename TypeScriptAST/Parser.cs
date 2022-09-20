using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Xml.Linq;
using TypeScriptAST.Declarations;
using TypeScriptAST.Declarations.Types;
using TypeScriptAST.Expressions;
using Array = TypeScriptAST.Expressions.Array;
using String = System.String;
using TemplateElement = TypeScriptAST.Expressions.TemplateLiteral.TemplateElement;
using Type = TypeScriptAST.Declarations.Types.Type;

namespace TypeScriptAST;

public class Parser
{
    private readonly TextParser _parser;
    private readonly ParserOptions _options;
    private Token _token;

    public Parser(TextParser parser, ParserOptions options)
    {
        _parser = parser;
        _options = options;
        NextToken(skipComments: false);
    }

    public Parser(string text, ParserOptions options) : this(new TextParser(text), options)
    {
    }

    public Parser(string text) : this(new TextParser(text))
    {
    }

    public Parser(TextParser parser) : this(parser, ParserOptions.GetDefault())
    {
    }

    public Statement Parse()
    {
        var statements = new List<Statement>();
        statements.AddRange(ResolveXmlComments());
        while (_token.Type is not TokenType.End)
        {
            var statement = ReadStatement();

            if (statement is Expression expression && statements.LastOrDefault() is Expression)
                throw new InvalidExpressionException(expression);

            statements.Add(statement);
        }
        return statements.Count == 1 ? statements[0] : new StatementList(statements);
    }

    private Statement? ResolveStatement()
    {
        return ResolveEmptyStatement()
            ?? (Statement?)ResolveExpression();
    }

    private IList<XmlComment> ResolveXmlComments()
    {
        bool IsXmlComment(string text) => text.StartsWith("///");

        var comments = new List<XmlComment>();
        while (_token.Type is TokenType.Comment)
        {
            if (IsXmlComment(_token.Text))
            {
                try
                {
                    var xml = XElement.Parse(_token.Text.Substring(3).Trim());
                    comments.Add(Statement.XmlComment(
                        _token.Text.Substring("///".Length),
                        xml.Name.LocalName,
                        xml.Attributes().ToDictionary(_ => _.Name.LocalName, _ => _.Value)));
                }
                catch (Exception ex)
                {
                    throw new InvalidTokenException("invalid xml comment format", _token, ex);
                }
            }
            NextToken(skipComments: false);
        }
        return comments;
    }

    private EmptyStatement? ResolveEmptyStatement()
    {
        if (_token.Type is not TokenType.Semicolon)
            return null;

        NextToken(); // ;

        return new EmptyStatement();
    }

    private Expression? ResolveExpression(OperatorType? priorOperator = null)
    {
        Expression? expression = null;
        while (_token.Type is not TokenType.End)
        {
            if (OperatorType.FromToken(_token, hasLeftOperand: expression is not null) is { } currentOperator)
            {
                if (priorOperator is not null)
                {
                    if (priorOperator != OperatorType.Grouping && currentOperator.Precedence <= priorOperator.Precedence)
                    {
                        break;
                    }
                }
            }
            else if (expression is not null)
            {
                break;
            }

            var newExpression =
                ResolveNumericLiteral()
                ?? ResolveStringLiteral()
                ?? ResolveRegExpLiteral()
                ?? ResolveTemplateLiteral()
                ?? ResolveKeywordsLiteral()
                ?? ResolveGroup(expression)
                ?? ResolveArray(expression)
                ?? ResolveUnaryPlusOrNegation(expression)
                ?? ResolveAddOrSubtract(expression)
                ?? ResolveMultiplication(expression)
                ?? ResolveDivision(expression)
                ?? ResolveRemainder(expression)
                ?? ResolveExponentiation(expression)
                ?? ResolveLogicalOperator(expression)
                ?? ResolveBitwiseOperator(expression)
                ?? ResolveComparisonOperator(expression)
                ?? ResolveNullishCoalescing(expression)
                ?? ResolveConditional(expression)
                ?? ResolveBuiltInOperators(expression)
                ?? ResolveNew()
                ?? ResolveIdentifier()
                ?? ResolveIncDecOperator(expression)
                ?? ResolveMemberAccess(expression)
                ?? ResolveMemberIndex(expression)
                ?? ResolveFunctionCall(expression, priorOperator: priorOperator);

            if (newExpression is not null)
            {
                expression = newExpression;
            }
            else
            {
                break;
            }
        }

        return expression;
    }

    private Literal? ResolveKeywordsLiteral()
    {
        switch (_token.Type)
        {
            case TokenType.NullLiteral:
                NextToken();
                return Expression.Literal(null, Type.Any);
            case TokenType.Identifier when _token.Text == "undefined":
                NextToken();
                return Expression.Literal(Undefined.Value, Type.Any);
            case TokenType.BooleanLiteral when _token.Text == "true":
                NextToken();
                return Expression.Literal(true, Type.Boolean);
            case TokenType.BooleanLiteral when _token.Text == "false":
                NextToken();
                return Expression.Literal(false, Type.Boolean);
            default:
                return null;
        }
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
                ApplyBigInt(Convert.ToInt64(text.Substring(2), fromBase: 16)),
            _ when isBin =>
                ApplyBigInt(Convert.ToInt64(text.Substring(2), fromBase: 2)),
            _ when isOct =>
                ApplyBigInt(Convert.ToInt64(text[1] is 'o' or 'O' ? text.Substring(2) : text.Substring(1), fromBase: 8)),
            _ when hasExp || isReal =>
                Convert.ToDouble(text),
            _ =>
                ApplyBigInt(Convert.ToInt64(text, fromBase: 10)),
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

                buffer.Append(EscapeCharacter);
                index++;
                continue;
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

    private TemplateLiteral? ResolveTemplateLiteral()
    {
        if (ResolveNoSubstitutionTemplateLiteral() is { } templateLiteral)
            return templateLiteral;

        if (_token.Type is not TokenType.TemplateHead)
            return null;

        var head = ReadTemplateElement(TokenType.TemplateHead);
        var elements = new List<TemplateElement> { head };
        while (_token.Type is not TokenType.End)
        {
            if (_token.Type is TokenType.TemplateMiddle)
            {
                elements.Add(ReadTemplateElement(TokenType.TemplateMiddle));
            }
            else if (_token.Type is TokenType.TemplateTail)
            {
                elements.Add(ReadTemplateElement(TokenType.TemplateTail));
                break;
            }
            else
            {
                elements.Add(ReadExpression());
            }
        }
        return Expression.TemplateLiteral(elements);
    }

    private TemplateElement ReadTemplateElement(TokenType type)
    {
        const char EscapeCharacter = '\\';

        var buffer = new StringBuilder();
        var lastIndex = _token.Text.Length - (type is TokenType.TemplateTail ? 2 : 3);
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
        return new TemplateElement(buffer.ToString());
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
        return Expression.TemplateLiteral(new[] { new TemplateElement(buffer.ToString()) });
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

    private Expression? ResolveIdentifier()
    {
        if (_token.Type is not TokenType.Identifier)
            return null;

        var name = "";
        while (_token.Type is TokenType.Identifier)
        {
            name = (name.Length > 0 ? name + "." : "") + _token.Text;

            if (GetName(name) is { } expression)
            {
                NextToken(); // identifier
                return expression;
            }

            if (_options.TypeSystem.NamespaceExists(name))
                NextToken(); // identifier
            else
                break;

            if (_token.Type is TokenType.Dot)
                NextToken(); // .
            else
                break;
        }

        return null;

        Expression? GetName(string name)
        {
            if (_options.TypeSystem.GetDeclaration(name).FirstOrDefault() is { } declaration)
            {
                return Expression.Identifier(declaration);
            }

            if (_options.TypeSystem.GetType(name) is { } type)
            {
                return Expression.Literal(type, type);
            }

            return null;
        }
    }

    private Type? ResolveType(OperatorType? priorOperator = null)
    {
        return ResolveExpression(priorOperator) is Literal { Value: Type type } ? type : null;
    }

    private Array? ResolveArray(Expression? left)
    {
        if (_token.Type is not TokenType.OpenBracket || left is not null)
            return null;

        NextToken(); // [
        var items = new List<Expression>();
        while (_token.Type is not TokenType.End)
        {
            if (_token.Type is TokenType.CloseBracket)
                break;

            items.Add(ReadExpression());

            if (_token.Type is TokenType.Comma)
                NextToken(); // ,
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

    private Expression? ResolveUnaryPlusOrNegation(Expression? left)
    {
        if (left is not null)
            return null;

        return _token.Type switch
        {
            TokenType.Plus => Expression.UnaryPlus(ReadNextExpression(OperatorType.UnaryPlus)),
            TokenType.Minus => Expression.Negate(ReadNextExpression(OperatorType.UnaryNegation)),
            _ => null
        };
    }

    private Expression? ResolveAddOrSubtract(Expression? left)
    {
        if (left is null)
            return null;

        return _token.Type switch
        {
            TokenType.Plus => Expression.Add(left, ReadNextExpression(OperatorType.Addition)),
            TokenType.Minus => Expression.Subtract(left, ReadNextExpression(OperatorType.Subtraction)),
            _ => null
        };
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

        if (_token.Type is not TokenType.Colon)
            throw new InvalidTokenException(_token);

        NextToken(); // :
        var falseExpression = ReadExpression(OperatorType.ConditionalTernary);
        return Expression.Conditional(condition, trueExpression, falseExpression);
    }

    private Expression? ResolveGroup(Expression? left)
    {
        if (_token.Type is not TokenType.OpenParen || left is not null)
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
            var member = TypeSystem.GetMember(left.Type, _token.Text).FirstOrDefault()
                         ?? throw new InvalidTokenException(_token);
            NextToken(); // identifier
            return Expression.MemberAccess(left, member, isOptional);
        }

        if (_token.Type is TokenType.OpenBracket)
        {
            return ResolveMemberIndex(left, isOptional);
        }

        if (_token.Type is TokenType.OpenParen)
        {
            return ResolveFunctionCall(left, isOptional);
        }

        throw new InvalidTokenException(_token);
    }

    private MemberIndex? ResolveMemberIndex(Expression? left, bool isOptional = false)
    {
        if (_token.Type is not TokenType.OpenBracket || left is null)
            return null;

        NextToken(); // [
        var index = ReadExpression(OperatorType.Grouping);

        if (_token.Type is not TokenType.CloseBracket)
            throw new InvalidTokenException(_token);

        NextToken(); // ]
        return Expression.MemberIndex(left, index, isOptional);
    }

    private Operator? ResolveFunctionCall(Expression? left, bool isOptional = false, OperatorType? priorOperator = null)
    {
        if (_token.Type is not TokenType.OpenParen || left is null)
            return null;

        var args = ReadArguments(OperatorType.FunctionCall);

        if (left is Identifier { Subject: FunctionDeclaration rawFunc })
        {
            var function =
                FindMatchingFunction(rawFunc, args)
                ?? throw new InvalidOperationException($"invalid function arguments: {rawFunc}, {args}");

            var expression = Expression.Identifier(function);
            return Expression.FunctionCall(expression, args, isOptional);
        }

        if (left is Identifier { Subject: VarDeclaration or ConstDeclaration or LetDeclaration,
            Subject: ModuleDeclaration { Type: FunctionType fType } declaration })
        {
            if (!IsMatchArguments(declaration, args))
                throw new InvalidOperationException($"invalid function arguments: {declaration}, {args}");

            var expression = Expression.Identifier(declaration);
            return Expression.FunctionCall(expression, args, fType.Type, isOptional);
        }

        if (left is MemberAccess { Member: FunctionMember functionMember } functionAccess)
        {
            var function =
                FindMatchingFunction(functionMember, args)
                ?? throw new InvalidOperationException($"invalid function arguments: {functionAccess.Member}, {args}");

            var expression = Expression.MemberAccess(functionAccess.Instance, function, functionAccess.IsOptional);
            return Expression.FunctionCall(expression, args, isOptional);
        }

        if (left is MemberAccess { Member: PropertyMember { Type: FunctionType pType } member } propertyAccess)
        {
            if (!IsMatchArguments(member, args))
                throw new InvalidOperationException($"invalid function arguments: {member}, {args}");

            return Expression.FunctionCall(propertyAccess, args, pType.Type, isOptional);
        }

        if (left is Literal { Value: Type type })
        {
            var function =
                TypeSystem.GetMembers(type, functionOnly: true).OfType<FunctionMember>()
                .Where(f => priorOperator == OperatorType.New ? f.IsConstructor : f.HasNoName)
                .FirstOrDefault(f => IsMatchArguments(f, args))
                ?? throw new InvalidOperationException($"invalid function arguments: {type}, {args}");

            var expression = Expression.MemberAccess(left, function, isOptional: false);
            return function.IsConstructor ?
                Expression.New(function, args) :
                Expression.FunctionCall(expression, args, isOptional);
        }

        throw new InvalidTokenException(_token);
    }

    private Expression[] ReadArguments(OperatorType? priorOperator)
    {
        if (_token.Type is not TokenType.OpenParen)
            throw new InvalidTokenException(_token);

        var args = new List<Expression>();
        NextToken(); // (
        while (_token.Type is not TokenType.End)
        {
            if (_token.Type is TokenType.CloseParen)
                break;

            args.Add(ReadExpression(priorOperator));

            if (_token.Type is TokenType.Comma)
                NextToken(); // ,
        }

        if (_token.Type is not TokenType.CloseParen)
            throw new InvalidTokenException(_token);

        NextToken(); // )

        return args.ToArray();
    }

    private IMemberInfo? FindMatchingFunction(IMemberInfo function, IReadOnlyList<Expression> args)
    {
        return _options.TypeSystem.GetMember(function.FullName)
               .FirstOrDefault(func => IsMatchArguments(func, args));
    }

    private bool IsMatchArguments(IMemberInfo function, IReadOnlyList<Expression> args, bool throwIfNotMatch = false)
    {
        var parameters =
            function is FunctionMember funcDef ? funcDef.Parameters :
            function is PropertyMember { Type: FunctionType pFunctionType } ? pFunctionType.Parameters :
            function is FunctionDeclaration funcDec ? funcDec.Parameters :
            function is ModuleDeclaration { Type: FunctionType dFunctionType } ? dFunctionType.Parameters :
            throw new ArgumentException($"member is not a function: {function.Name}", nameof(function));

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
                var restParamIndex = parameters.ToList().FindIndex(_ => _.IsRest);
                return restParamIndex >= 0 && index >= restParamIndex
                    ? TypeSystem.GetElementType(parameters[restParamIndex].Type)
                    : parameters[index].Type;
            }

            return args
                .Select((arg, index) => (Index: index, Type: arg.Type)).ToList()
                .All(arg => ResolveParameterType(arg.Index).IsAssignableFrom(arg.Type));
        }
    }

    private Operator? ResolveBuiltInOperators(Expression? left)
    {
        if (_token.Type is not TokenType.Identifier)
            return null;

        switch (_token.Text)
        {
            case "void":
                return Expression.VoidOf(ReadNextExpression(OperatorType.VoidOf));

            case "instanceof":
                if (left is null)
                    throw new InvalidTokenException(_token);

                return Expression.InstanceOf(left, ReadNextExpression(OperatorType.InstanceOf));

            case "in":
                if (left is null)
                    throw new InvalidTokenException(_token);

                return Expression.In(left, ReadNextExpression(OperatorType.In));

            case "typeof":
                return Expression.TypeOf(ReadNextExpression(OperatorType.TypeOf));

            case "as":
                if (left is null)
                    throw new InvalidTokenException(_token);

                NextToken(); // as
                var targetType =
                    ResolveType(OperatorType.As)
                    ?? throw new InvalidTokenException(_token);
                return Expression.As(left, targetType);

            default:
                return null;
        }
    }

    private New? ResolveNew()
    {
        if (_token.Type is not TokenType.Identifier || _token.Text != "new")
            return null;

        var newExpression = ReadNextExpression(OperatorType.New);
        return newExpression as New
               ?? throw new InvalidOperandException(OperatorType.New, newExpression);
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

    private Expression ReadExpression(OperatorType? priorOperator = null)
    {
        return ResolveExpression(priorOperator)
               ?? throw new InvalidTokenException(_token);
    }

    private Expression ReadNextExpression(OperatorType? priorOperator = null, bool skipComments = true)
    {
        NextToken(skipComments);
        return ReadExpression(priorOperator);
    }

    private Statement ReadStatement()
    {
        return ResolveStatement()
               ?? throw new InvalidTokenException(_token);
    }

    private Statement ReadNextStatement(bool skipComments = true)
    {
        NextToken(skipComments);
        return ReadStatement();
    }

    private bool IsLineTerminator(char character)
    {
        return character is '\u000A' or '\u2028' or '\u2029' or '\u000D';
    }
}
