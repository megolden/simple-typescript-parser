using System;

namespace TypeScriptAST;

public class TextParser
{
    private const char NumericLiteralSeparator = '_';

    private readonly string _text;
    private readonly int _textLength;
    private int _offset;
    private char _char;
    private Token? _previousNonCommentToken;
    private int _templateDepth;

    public string Text => _text;

    public TextParser(string text)
    {
        if (text is null)
            throw new ArgumentNullException(nameof(text));

        _text = text;
        _textLength = text.Length;
        _offset = 0;
        _char = text.Length > 0 ? text[0] : '\0';
        _previousNonCommentToken = null;
        _templateDepth = 0;
    }

    public Token ReadToken()
    {
        while (IsWhitespace()) NextChar();

        var token =
            ReadEnd()
         ?? ReadNumeric()
         ?? ReadTemplateOrHead()
         ?? ReadTemplateMiddleOrTail()
         ?? ReadDivisionOps()
         ?? ReadRegExp()
         ?? ReadString()
         ?? ReadIdentifier()
         ?? ReadComment()
         ?? ReadEqualityOps()
         ?? ReadBitwiseShiftOps()
         ?? ReadIncDecExpOps()
         ?? ReadComparisonOps()
         ?? ReadBitwiseOps()
         ?? ReadContextualOps()
         ?? ReadAssignmentOps()
         ?? ReadArithmeticOps()
         ?? ReadUnknown();

        if (token.Type is TokenType.TemplateHead)
            _templateDepth++;
        if (token.Type is TokenType.TemplateTail)
            _templateDepth--;

        if (token.Type is not TokenType.Comment)
            _previousNonCommentToken = token;

        return token;
    }

    private bool IsEnd()
    {
        return _offset == _textLength;
    }

    private bool IsLetter(char character)
    {
        return (character is >= 'a' and <= 'z') || (character is >= 'A' and <= 'Z');
    }

    private bool IsLineTerminator(out int len)
    {
        if (_char is '\u000A' or '\u2028' or '\u2029')
        {
            len = 1;
            return true;
        }

        if (_char is '\u000D')
        {
            len = PeekNextChar() is '\u000A' ? 2 : 1;
            return true;
        }

        len = default;
        return false;
    }

    private bool IsLineTerminator() => IsLineTerminator(out _);

    private bool IsWhitespace()
    {
        return _char
            is '\u0009' // tab
            or '\u000B' // line tabulation
            or '\u000C' // form feed
            or '\u0020' // space
            or '\u00A0' // no-break space
            or '\u000A' // line feed
            or '\u000D' // carriage return
            or '\uFEFF' // zw no-break space
            || Char.IsWhiteSpace(_char);
    }

    private bool IsDigit(char character)
    {
        return character is >= '0' and <= '9';
    }

    private bool IsDigit() => IsDigit(_char);

    private bool IsBinaryDigit() => _char is '0' or '1';

    private bool IsOctalDigit() => _char is >= '0' and <= '7';

    private bool IsHexDigit() => _char is (>= '0' and <= '9') or (>= 'a' and <= 'f') or (>= 'A' and <= 'F');

    private bool IsValidIdentifierChar(char character, bool isFirstLetter)
    {
        return character is '_' or '$' || IsLetter(character) || (IsDigit(character) && !isFirstLetter);
    }

    private void NextChar(int count = 1)
    {
        if (IsEnd()) return;

        _offset += count;
        if (_offset > _textLength) _offset = _textLength;

        _char = IsEnd() ? '\0' : _text[_offset];
    }

    private char PeekChar(int index)
    {
        return index >= 0 && index < _textLength ? _text[index] : '\0';
    }

    private char PeekNextChar() => PeekChar(_offset + 1);

    private Token? ReadComment()
    {
        var isValidCommentStart = _char is '/' && PeekNextChar() is '/' or '*';
        if (!isValidCommentStart) return null;

        var startPos = _offset;
        NextChar(); // /
        var isMultiLine = _char is '*';
        if (isMultiLine) NextChar(); // *
        var isSingleLine = !isMultiLine;

        while (!IsEnd())
        {
            if (isSingleLine && IsLineTerminator())
            {
                break;
            }

            if (isMultiLine && _char is '*' && PeekNextChar() is '/')
            {
                NextChar(2);
                break;
            }

            NextChar();
        }

        var text = _text.Substring(startPos, _offset - startPos);

        if (isMultiLine)
        {
            if (text.Length < 4) // /**/
                throw new InvalidTokenException(UnknownToken());
            if (!text.EndsWith("*/"))
                throw new InvalidTokenException(UnknownToken());
        }

        return Token.Comment(text, startPos);
    }

    private Token? ReadNumeric()
    {
        return ReadHexInteger()
            ?? ReadOctalInteger()
            ?? ReadBinaryInteger()
            ?? ReadDecimalInteger();
    }

    private Token? ReadBinaryInteger()
    {
        var isValidStart = _char is '0' && PeekNextChar() is 'b' or 'B';
        if (!isValidStart) return null;

        var startPos = _offset;
        NextChar(2); // 0b or 0B

        var prevChar = '\0'; // start
        var hasBigIntSuffix = false;
        while (!IsEnd())
        {
            if (IsBinaryDigit())
            {
                prevChar = _char;
                NextChar();
                continue;
            }

            if (_char is NumericLiteralSeparator)
            {
                if (prevChar is '\0' or NumericLiteralSeparator)
                    break;

                prevChar = _char;
                NextChar();
                continue;
            }

            if (_char is 'n')
            {
                if (prevChar is '\0' or NumericLiteralSeparator)
                    break;

                NextChar();
                hasBigIntSuffix = true;
                break;
            }

            break;
        }

        var text = _text.Substring(startPos, _offset - startPos);

        if (text.Length <= (hasBigIntSuffix ? 3 : 2))
            throw new InvalidTokenException(UnknownToken());

        if (text[text.Length - 1] is '_' || (hasBigIntSuffix && text[text.Length - 2] is '_'))
            throw new InvalidTokenException(UnknownToken());

        return Token.NumericLiteral(text, startPos);
    }

    private Token? ReadOctalInteger()
    {
        var isValidStart = _char is '0' && PeekNextChar() is 'o' or 'O' or (>= '0' and <= '7');
        if (!isValidStart) return null;

        var startPos = _offset;
        var hasOctalPrefix = false;
        NextChar(); // 0
        if (_char is 'o' or 'O')
        {
            hasOctalPrefix = true;
            NextChar();
        }

        var prevChar = '\0'; // start
        var hasBigIntSuffix = false;
        while (!IsEnd())
        {
            if (IsOctalDigit())
            {
                prevChar = _char;
                NextChar();
                continue;
            }

            if (_char is NumericLiteralSeparator)
            {
                if (prevChar is '\0' or NumericLiteralSeparator)
                    break;

                prevChar = _char;
                NextChar();
                continue;
            }

            if (_char is 'n')
            {
                if (prevChar is '\0' or NumericLiteralSeparator)
                    break;

                NextChar();
                hasBigIntSuffix = true;
                break;
            }

            break;
        }

        var text = _text.Substring(startPos, _offset - startPos);

        if (hasOctalPrefix)
        {
            if (text.Length <= (hasBigIntSuffix ? 3 : 2))
                throw new InvalidTokenException(UnknownToken());

            if (text[text.Length - 1] is '_' || (hasBigIntSuffix && text[text.Length - 2] is '_'))
                throw new InvalidTokenException(UnknownToken());
        }
        else
        {
            if (text.Length <= (hasBigIntSuffix ? 3 : 2))
                throw new InvalidTokenException(UnknownToken());

            if (text[text.Length - 1] is '_' || (hasBigIntSuffix && text[text.Length - 2] is '_'))
                throw new InvalidTokenException(UnknownToken());
        }

        return Token.NumericLiteral(text, startPos);
    }

    private Token? ReadDecimalInteger()
    {
        const char DecimalSeparator = '.';

        var isValidStart = IsDigit() || (_char is DecimalSeparator && IsDigit(PeekNextChar()));
        if (!isValidStart) return null;

        void ReadDecimalDigits(bool allowBigIntSuffix, out bool bigInt)
        {
            var prevChar = '\0';
            bigInt = false;
            while (!IsEnd())
            {
                if (_char is NumericLiteralSeparator)
                {
                    if (prevChar is '\0' or NumericLiteralSeparator)
                        break;

                    prevChar = _char;
                    NextChar();
                    continue;
                }

                if (_char is 'n' && allowBigIntSuffix)
                {
                    if (prevChar is '\0' or NumericLiteralSeparator)
                        break;

                    bigInt = true;
                    NextChar();
                    break;
                }

                if (IsDigit())
                {
                    prevChar = _char;
                    NextChar();
                    continue;
                }

                break;
            }
        }

        void ReadExponentPart()
        {
            if (_char is not 'e' and not 'E')
                return;

            NextChar();
            if (_char is '+' or '-')
                NextChar();

            ReadDecimalDigits(allowBigIntSuffix: false, out _);
        }

        var startPos = _offset;
        if (_char is DecimalSeparator)
        {
            NextChar(); // decimal separator
            ReadDecimalDigits(allowBigIntSuffix: false, out _);
            ReadExponentPart();
        }
        else
        {
            ReadDecimalDigits(allowBigIntSuffix: true, out var bigInt);
            if (!bigInt)
            {
                if (_char is DecimalSeparator)
                {
                    NextChar(); // decimal separator
                    ReadDecimalDigits(allowBigIntSuffix: false, out _);
                }
                ReadExponentPart();
            }
        }

        return Token.NumericLiteral(_text.Substring(startPos, _offset - startPos), startPos);
    }

    private Token? ReadHexInteger()
    {
        var isValidStart = _char is '0' && PeekNextChar() is 'x' or 'X';
        if (!isValidStart) return null;

        var startPos = _offset;
        NextChar(2); // 0x or 0X

        var prevChar = '\0'; // start
        var hasBigIntSuffix = false;
        while (!IsEnd())
        {
            if (IsHexDigit())
            {
                prevChar = _char;
                NextChar();
                continue;
            }

            if (_char is NumericLiteralSeparator)
            {
                if (prevChar is '\0' or NumericLiteralSeparator)
                    break;

                prevChar = _char;
                NextChar();
                continue;
            }

            if (_char is 'n')
            {
                if (prevChar is '\0' or NumericLiteralSeparator)
                    break;

                NextChar();
                hasBigIntSuffix = true;
                break;
            }

            break;
        }

        var text = _text.Substring(startPos, _offset - startPos);

        if (text.Length <= (hasBigIntSuffix ? 3 : 2))
            throw new InvalidTokenException(UnknownToken());

        if (text[text.Length - 1] is '_' || (hasBigIntSuffix && text[text.Length - 2] is '_'))
            throw new InvalidTokenException(UnknownToken());

        return Token.NumericLiteral(text, startPos);
    }

    private Token? ReadString()
    {
        if (_char is not '\'' and not '"') return null;

        const char EscapeCharacter = '\\';

        var startPos = _offset;
        var startSymbol = _char;
        NextChar(); // " or '
        while (!IsEnd())
        {
            if (_char is EscapeCharacter)
            {
                NextChar(); // escape char
                if (IsLineTerminator(out var count))
                    NextChar(count); // line end
                else
                    NextChar(); // other char
            }

            if (IsLineTerminator())
            {
                break;
            }

            if (_char == startSymbol)
            {
                NextChar(); // ' or "
                break;
            }

            NextChar();
        }

        var text = _text.Substring(startPos, _offset - startPos);

        if (text.Length < 2)
            throw new InvalidTokenException(UnknownToken());
        if (!text.EndsWith(startSymbol))
            throw new InvalidTokenException(UnknownToken());
        if (text[text.Length - 2] is EscapeCharacter)
            throw new InvalidTokenException(UnknownToken());

        return Token.StringLiteral(text, startPos);
    }

    private Token? ReadRegExp()
    {
        if (_char is not '/' ||
            (PeekNextChar() is '/' or '*') ||
            _previousNonCommentToken?.Type
            is TokenType.Identifier
            or TokenType.NumericLiteral
            or TokenType.NullLiteral
            or TokenType.BooleanLiteral)
        {
            return null;
        }

        bool ReadFlags()
        {
            ushort flags = 0; // d g i m s u y
            while (!IsEnd())
            {
                if (_char is 'd' && (flags & 1) == 0)
                {
                    flags |= 1;
                    NextChar();
                    continue;
                }
                if (_char is 'g' && (flags & 2) == 0)
                {
                    flags |= 2;
                    NextChar();
                    continue;
                }
                if (_char is 'i' && (flags & 4) == 0)
                {
                    flags |= 4;
                    NextChar();
                    continue;
                }
                if (_char is 'm' && (flags & 8) == 0)
                {
                    flags |= 8;
                    NextChar();
                    continue;
                }
                if (_char is 's' && (flags & 16) == 0)
                {
                    flags |= 16;
                    NextChar();
                    continue;
                }
                if (_char is 'u' && (flags & 32) == 0)
                {
                    flags |= 32;
                    NextChar();
                    continue;
                }
                if (_char is 'y' && (flags & 64) == 0)
                {
                    flags |= 64;
                    NextChar();
                    continue;
                }

                break;
            }
            return flags != 0;
        }

        const char EscapeCharacter = '\\';

        var startPos = _offset;
        var hasFlags = false;
        NextChar(); // /
        while (!IsEnd())
        {
            if (_char is EscapeCharacter)
            {
                NextChar(); // escape char
                if (_char is '/')
                {
                    NextChar(); // /
                    continue;
                }
                else
                {
                    continue;
                }
            }

            if (_char is '/')
            {
                NextChar(); // /
                hasFlags = ReadFlags();
                break;
            }

            if (IsLineTerminator())
                break;

            NextChar();
        }

        var text = _text.Substring(startPos, _offset - startPos);

        if (text.Length < 3)
            throw new InvalidTokenException(UnknownToken());
        if (!hasFlags && !text.EndsWith('/'))
            throw new InvalidTokenException(UnknownToken());

        return Token.RegExpLiteral(text, startPos);
    }

    private Token? ReadTemplateOrHead()
    {
        if (_char is not '`') return null;

        const char EscapeCharacter = '\\';

        var startPos = _offset;
        var isTemplateHead = false;
        NextChar(); // `
        while (!IsEnd())
        {
            if (_char is EscapeCharacter)
            {
                NextChar(); // escape char
                if (IsLineTerminator(out var count))
                {
                    NextChar(count); // line end
                }
                else
                {
                    NextChar(); // other char
                }
                continue;
            }

            if (_char == '`')
            {
                NextChar(); // `
                break;
            }

            if (_char is '$' && PeekNextChar() is '{')
            {
                isTemplateHead = true;
                NextChar(2);
                break;
            }

            NextChar(); // any char
        }

        var text = _text.Substring(startPos, _offset - startPos);

        if (isTemplateHead)
        {
            return Token.TemplateHead(text, startPos);
        }
        else
        {
            if (text.Length < 2)
                throw new InvalidTokenException(UnknownToken());
            if (!text.EndsWith('`'))
                throw new InvalidTokenException(UnknownToken());
            if (text[text.Length - 2] is EscapeCharacter)
                throw new InvalidTokenException(UnknownToken());

            return Token.TemplateLiteral(text, startPos);
        }
    }

    private Token? ReadTemplateMiddleOrTail()
    {
        if (_char is not '}' || _templateDepth == 0) return null;

        if (_previousNonCommentToken?.Type is TokenType.TemplateHead) // empty substitution
            throw new InvalidTokenException(UnknownToken());

        const char EscapeCharacter = '\\';

        var startPos = _offset;
        var isTemplateMiddle = false;
        NextChar(); // }
        while (!IsEnd())
        {
            if (_char is EscapeCharacter)
            {
                NextChar(); // escape char
                if (IsLineTerminator(out var count))
                {
                    NextChar(count); // line end
                }
                else
                {
                    NextChar(); // other char
                }
                continue;
            }

            if (_char == '`')
            {
                NextChar(); // `
                break;
            }

            if (_char is '$' && PeekNextChar() is '{')
            {
                isTemplateMiddle = true;
                NextChar(2);
                break;
            }

            NextChar(); // any char
        }

        var text = _text.Substring(startPos, _offset - startPos);

        if (isTemplateMiddle)
        {
            return Token.TemplateMiddle(text, startPos);
        }
        else
        {
            if (text.Length < 2)
                throw new InvalidTokenException(UnknownToken());
            if (!text.EndsWith('`'))
                throw new InvalidTokenException(UnknownToken());
            if (text[text.Length - 2] is EscapeCharacter)
                throw new InvalidTokenException(UnknownToken());

            return Token.TemplateTail(text, startPos);
        }
    }

    private Token? ReadIdentifier()
    {
        var isValidIdentifierStart = IsValidIdentifierChar(_char, isFirstLetter: true);
        if (!isValidIdentifierStart) return null;

        var startPos = _offset;
        do
        {
            NextChar();

            if (!IsValidIdentifierChar(_char, isFirstLetter: false))
                break;

        } while (!IsEnd());

        var text = _text.Substring(startPos, _offset - startPos);

        if (text == "null")
            return Token.NullLiteral(startPos);
        if (text == "true")
            return Token.TrueLiteral(startPos);
        if (text == "false")
            return Token.FalseLiteral(startPos);

        return Token.Identifier(text, startPos);
    }

    private Token? ReadEqualityOps()
    {
        Token? token = null;
        var startPos = _offset;
        switch (_char)
        {
            case '=' when PeekNextChar() is '=':
                NextChar(2);
                if (_char is '=')
                {
                    token = Token.TripleEqual(startPos);
                    NextChar();
                }
                else
                {
                    token = Token.DoubleEqual(startPos);
                }
                break;
            case '!' when PeekNextChar() is '=':
                NextChar(2);
                if (_char is '=')
                {
                    token = Token.ExclamationDoubleEqual(startPos);
                    NextChar();
                }
                else
                {
                    token = Token.ExclamationEqual(startPos);
                }
                break;
            case '&' when PeekNextChar() is '=':
                token = Token.AmpersandEqual(startPos);
                NextChar(2);
                break;
        }
        return token;
    }

    private Token? ReadBitwiseShiftOps()
    {
        Token? token = null;
        var startPos = _offset;
        switch (_char)
        {
            case '>' when PeekNextChar() is '>':
                NextChar(2);
                if (_char is '>')
                {
                    NextChar();
                    if (_char is '=')
                    {
                        token = Token.TripleGreaterThanEqual(startPos);
                        NextChar();
                    }
                    else
                    {
                        token = Token.TripleGreaterThan(startPos);
                    }
                }
                else if (_char is '=')
                {
                    token = Token.DoubleGreaterThanEqual(startPos);
                    NextChar();
                }
                else
                {
                    token = Token.DoubleGreaterThan(startPos);
                }
                break;
            case '<' when PeekNextChar() is '<':
                NextChar(2);
                if (_char is '=')
                {
                    token = Token.DoubleLessThanEqual(startPos);
                    NextChar();
                }
                else
                {
                    token = Token.DoubleLessThan(startPos);
                }
                break;
        }
        return token;
    }

    private Token? ReadIncDecExpOps()
    {
        Token? token = null;
        var startPos = _offset;
        switch (_char)
        {
            case '+' when PeekNextChar() is '+':
                token = Token.DoublePlus(startPos);
                NextChar(2);
                break;
            case '-' when PeekNextChar() is '-':
                token = Token.DoubleMinus(startPos);
                NextChar(2);
                break;
            case '*' when PeekNextChar() is '*':
                NextChar(2);
                if (_char is '=')
                {
                    token = Token.DoubleAsteriskEqual(startPos);
                    NextChar();
                }
                else
                {
                    token = Token.DoubleAsterisk(startPos);
                }
                break;
        }
        return token;
    }

    private Token? ReadComparisonOps()
    {
        Token? token = null;
        var startPos = _offset;
        switch (_char)
        {
            case '>':
                NextChar();
                if (_char is '=')
                {
                    token = Token.GreaterThanEqual(startPos);
                    NextChar();
                }
                else
                {
                    token = Token.GreaterThan(startPos);
                }
                break;
            case '<':
                NextChar();
                if (_char is '=')
                {
                    token = Token.LessThanEqual(startPos);
                    NextChar();
                }
                else
                {
                    token = Token.LessThan(startPos);
                }
                break;
            case '&' when PeekNextChar() is '&':
                NextChar(2);
                if (_char is '=')
                {
                    token = Token.DoubleAmpersandEqual(startPos);
                    NextChar();
                }
                else
                {
                    token = Token.DoubleAmpersand(startPos);
                }
                break;
            case '|' when PeekNextChar() is '|':
                NextChar(2);
                if (_char is '=')
                {
                    token = Token.DoubleBarEqual(startPos);
                    NextChar();
                }
                else
                {
                    token = Token.DoubleBar(startPos);
                }
                break;
            case '|' when PeekNextChar() is '=':
                token = Token.BarEqual(startPos);
                NextChar(2);
                break;
            case '!':
                token = Token.Exclamation(startPos);
                NextChar();
                break;
        }
        return token;
    }

    private Token? ReadBitwiseOps()
    {
        Token? token = null;
        switch (_char)
        {
            case '&':
                token = Token.Ampersand(_offset);
                NextChar();
                break;
            case '|':
                token = Token.Bar(_offset);
                NextChar();
                break;
            case '^' when PeekNextChar() is '=':
                token = Token.CaratEqual(_offset);
                NextChar(2);
                break;
            case '^':
                token = Token.Carat(_offset);
                NextChar();
                break;
            case '~':
                token = Token.Tilde(_offset);
                NextChar();
                break;
        }
        return token;
    }

    private Token? ReadContextualOps()
    {
        Token? token = null;
        var startPos = _offset;
        switch (_char)
        {
            case ':':
                token = Token.Colon(startPos);
                NextChar();
                break;
            case ',':
                token = Token.Comma(startPos);
                NextChar();
                break;
            case ';':
                token = Token.Semicolon(startPos);
                NextChar();
                break;
            case '.':
                NextChar();
                if (_char is '.' && PeekNextChar() is '.')
                {
                    token = Token.TripleDot(startPos);
                    NextChar(2);
                }
                else
                {
                    token = Token.Dot(startPos);
                }
                break;
            case '[':
                token = Token.OpenBracket(startPos);
                NextChar();
                break;
            case ']':
                token = Token.CloseBracket(startPos);
                NextChar();
                break;
            case '(':
                token = Token.OpenParen(startPos);
                NextChar();
                break;
            case ')':
                token = Token.CloseParen(startPos);
                NextChar();
                break;
            case '{':
                token = Token.OpenBrace(startPos);
                NextChar();
                break;
            case '}':
                token = Token.CloseBrace(startPos);
                NextChar();
                break;
            case '=' when PeekNextChar() is '>':
                token = Token.EqualGreaterThan(startPos);
                NextChar(2);
                break;
            case '?':
                NextChar();
                if (_char is '?')
                {
                    NextChar();
                    if (_char is '=')
                    {
                        token = Token.DoubleQuestionEqual(startPos);
                        NextChar();
                    }
                    else
                    {
                        token = Token.DoubleQuestion(startPos);
                    }
                }
                else if (_char is '.')
                {
                    token = Token.QuestionDot(startPos);
                    NextChar();
                }
                else
                {
                    token = Token.Question(startPos);
                }
                break;
        }
        return token;
    }

    private Token? ReadDivisionOps()
    {
        if (_char is not '/') return null;

        var nextChar = PeekNextChar();

        if (_previousNonCommentToken?.Type is not(
            TokenType.Identifier
            or TokenType.CloseParen
            or TokenType.CloseBracket
            or TokenType.DoublePlus
            or TokenType.DoubleMinus
            or TokenType.NullLiteral
            or TokenType.StringLiteral
            or TokenType.TemplateLiteral
            or TokenType.RegExpLiteral
            or TokenType.NumericLiteral
            or TokenType.BooleanLiteral))
            return null;

        Token? token = null;
        if (nextChar is '=')
        {
            token = Token.SlashEqual(_offset);
            NextChar(2);
        }
        else if (nextChar is not '/' and not '*')
        {
            token = Token.Slash(_offset);
            NextChar();
        }

        return token;
    }

    private Token? ReadAssignmentOps()
    {
        Token? token = null;
        switch (_char)
        {
            case '=':
                token = Token.Equal(_offset);
                NextChar();
                break;
            case '+' when PeekNextChar() is '=':
                token = Token.PlusEqual(_offset);
                NextChar(2);
                break;
            case '-' when PeekNextChar() is '=':
                token = Token.MinusEqual(_offset);
                NextChar(2);
                break;
            case '*' when PeekNextChar() is '=':
                token = Token.AsteriskEqual(_offset);
                NextChar(2);
                break;
            case '%' when PeekNextChar() is '=':
                token = Token.PercentEqual(_offset);
                NextChar(2);
                break;
        }
        return token;
    }

    private Token? ReadArithmeticOps()
    {
        Token? token = null;
        switch (_char)
        {
            case '+':
                token = Token.Plus(_offset);
                NextChar();
                break;
            case '-':
                token = Token.Minus(_offset);
                NextChar();
                break;
            case '*':
                token = Token.Asterisk(_offset);
                NextChar();
                break;
            case '%':
                token = Token.Percent(_offset);
                NextChar();
                break;
        }
        return token;
    }

    private Token? ReadEnd()
    {
        return IsEnd() ? Token.End(_textLength) : null;
    }

    private Token ReadUnknown()
    {
        var token = UnknownToken();
        NextChar();
        return token;
    }

    private Token UnknownToken()
    {
        var text = IsEnd() ? String.Empty : _char.ToString();
        return Token.Unknown(text, _offset);
    }

    public override string ToString()
    {
        return _text + (_offset >= 0 ? $" ({_offset}:{_text[_offset]})" : "");
    }
}
