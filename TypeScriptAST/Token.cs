using System;

namespace TypeScriptAST;

public sealed class Token
{
    private Token(TokenType type, string text, int position)
    {
        Type = type;
        Text = text;
        Position = position;
    }

    public override string ToString()
    {
        return Type switch
        {
            TokenType.Identifier when Text == "new" => "new ",
            TokenType.Unknown => "UNKNOWN",
            TokenType.End => "END",
            _ => Text.Length == 0 ? Type.ToString() : Text
        };
    }

    public string ToText()
    {
        return Type switch
        {
            TokenType.Identifier when Text == "new" => "new ",
            TokenType.End => String.Empty,
            _ => Text
        };
    }

    public override bool Equals(object? obj)
    {
        return obj switch
        {
            Token that =>
                this.Type == that.Type &&
                this.Position == that.Position &&
                this.Text == that.Text,
            _ => false
        };
    }

    public override int GetHashCode()
    {
        return (Type, Position, Text).GetHashCode();
    }

    public int Position { get; private init; }
    public string Text { get; private init; }
    public TokenType Type { get; private init; }

    public static Token NullLiteral(int position) => new (TokenType.NullLiteral, "null", position);
    public static Token TrueLiteral(int position) => new (TokenType.BooleanLiteral, "true", position);
    public static Token FalseLiteral(int position) => new (TokenType.BooleanLiteral, "false", position);
    public static Token NumericLiteral(string text, int position) => new (TokenType.NumericLiteral, text, position);
    public static Token StringLiteral(string text, int position) => new (TokenType.StringLiteral, text, position);
    public static Token TemplateLiteral(string text, int position) => new (TokenType.TemplateLiteral, text, position);
    public static Token TemplateHead(string text, int position) => new (TokenType.TemplateHead, text, position);
    public static Token TemplateMiddle(string text, int position) => new (TokenType.TemplateMiddle, text, position);
    public static Token TemplateTail(string text, int position) => new (TokenType.TemplateTail, text, position);
    public static Token RegExpLiteral(string text, int position) => new (TokenType.RegExpLiteral, text, position);
    public static Token Identifier(string text, int position) => new (TokenType.Identifier, text, position);
    public static Token Comment(string text, int position) => new (TokenType.Comment, text, position);
    public static Token Plus(int position) => new (TokenType.Plus, "+", position);
    public static Token DoublePlus(int position) => new (TokenType.DoublePlus, "++", position);
    public static Token PlusEqual(int position) => new (TokenType.PlusEqual, "+=", position);
    public static Token Minus(int position) => new (TokenType.Minus, "-", position);
    public static Token DoubleMinus(int position) => new (TokenType.DoubleMinus, "--", position);
    public static Token MinusEqual(int position) => new (TokenType.MinusEqual, "-=", position);
    public static Token Asterisk(int position) => new (TokenType.Asterisk, "*", position);
    public static Token AsteriskEqual(int position) => new (TokenType.AsteriskEqual, "*=", position);
    public static Token DoubleAsterisk(int position) => new (TokenType.DoubleAsterisk, "**", position);
    public static Token DoubleAsteriskEqual(int position) => new (TokenType.DoubleAsteriskEqual, "**=", position);
    public static Token Slash(int position) => new (TokenType.Slash, "/", position);
    public static Token SlashEqual(int position) => new (TokenType.SlashEqual, "/=", position);
    public static Token Exclamation(int position) => new (TokenType.Exclamation, "!", position);
    public static Token ExclamationEqual(int position) => new (TokenType.ExclamationEqual, "!=", position);
    public static Token ExclamationDoubleEqual(int position) => new (TokenType.ExclamationDoubleEqual, "!==", position);
    public static Token Percent(int position) => new (TokenType.Percent, "%", position);
    public static Token PercentEqual(int position) => new (TokenType.PercentEqual, "%=", position);
    public static Token Ampersand(int position) => new (TokenType.Ampersand, "&", position);
    public static Token DoubleAmpersand(int position) => new (TokenType.DoubleAmpersand, "&&", position);
    public static Token AmpersandEqual(int position) => new (TokenType.AmpersandEqual, "&=", position);
    public static Token DoubleAmpersandEqual(int position) => new (TokenType.DoubleAmpersandEqual, "&&=", position);
    public static Token Bar(int position) => new (TokenType.Bar, "|", position);
    public static Token DoubleBar(int position) => new (TokenType.DoubleBar, "||", position);
    public static Token BarEqual(int position) => new (TokenType.BarEqual, "|=", position);
    public static Token DoubleBarEqual(int position) => new (TokenType.DoubleBarEqual, "||=", position);
    public static Token Comma(int position) => new (TokenType.Comma, ",", position);
    public static Token Equal(int position) => new (TokenType.Equal, "=", position);
    public static Token DoubleEqual(int position) => new (TokenType.DoubleEqual, "==", position);
    public static Token EqualGreaterThan(int position) => new (TokenType.EqualGreaterThan, "=>", position);
    public static Token TripleEqual(int position) => new (TokenType.TripleEqual, "===", position);
    public static Token LessThan(int position) => new (TokenType.LessThan, "<", position);
    public static Token LessThanEqual(int position) => new (TokenType.LessThanEqual, "<=", position);
    public static Token DoubleLessThan(int position) => new (TokenType.DoubleLessThan, "<<", position);
    public static Token DoubleLessThanEqual(int position) => new (TokenType.DoubleLessThanEqual, "<<=", position);
    public static Token GreaterThan(int position) => new (TokenType.GreaterThan, ">", position);
    public static Token GreaterThanEqual(int position) => new (TokenType.GreaterThanEqual, ">=", position);
    public static Token DoubleGreaterThan(int position) => new (TokenType.DoubleGreaterThan, ">>", position);
    public static Token DoubleGreaterThanEqual(int position) => new (TokenType.DoubleGreaterThanEqual, ">>=", position);
    public static Token TripleGreaterThan(int position) => new (TokenType.TripleGreaterThan, ">>>", position);
    public static Token TripleGreaterThanEqual(int position) => new (TokenType.TripleGreaterThanEqual, ">>>=", position);
    public static Token OpenParen(int position) => new (TokenType.OpenParen, "(", position);
    public static Token CloseParen(int position) => new (TokenType.CloseParen, ")", position);
    public static Token OpenBrace(int position) => new (TokenType.OpenBrace, "{", position);
    public static Token CloseBrace(int position) => new (TokenType.CloseBrace, "}", position);
    public static Token OpenBracket(int position) => new (TokenType.OpenBracket, "[", position);
    public static Token CloseBracket(int position) => new (TokenType.CloseBracket, "]", position);
    public static Token Dot(int position) => new (TokenType.Dot, ".", position);
    public static Token TripleDot(int position) => new (TokenType.TripleDot, "...", position);
    public static Token Colon(int position) => new (TokenType.Colon, ":", position);
    public static Token Semicolon(int position) => new (TokenType.Semicolon, ";", position);
    public static Token DoubleQuestion(int position) => new (TokenType.DoubleQuestion, "??", position);
    public static Token DoubleQuestionEqual(int position) => new (TokenType.DoubleQuestionEqual, "??=", position);
    public static Token QuestionDot(int position) => new (TokenType.QuestionDot, "?.", position);
    public static Token Question(int position) => new (TokenType.Question, "?", position);
    public static Token Tilde(int position) => new (TokenType.Tilde, "~", position);
    public static Token Carat(int position) => new (TokenType.Carat, "^", position);
    public static Token CaratEqual(int position) => new (TokenType.CaratEqual, "^=", position);
    public static Token Unknown(string text, int position) => new (TokenType.Unknown, text, position);
    public static Token End(int position) => new (TokenType.End, String.Empty, position);
}
