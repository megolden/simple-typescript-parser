using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace TypeScriptAST.Tests;

public class TextParserTests
{
    [Fact]
    void Empty_expression_should_parse_single_End_token_only()
    {
        var tokens = TokenizeUsingParser(String.Empty);

        tokens.Should().HaveCount(1);
        tokens.Should().Contain(Token.End(0));
    }

    [Theory]
    [InlineData("\u0009")] // tab
    [InlineData("\u000B")] // line tabulation
    [InlineData("\u000C")] // form feed
    [InlineData("\u0020")] // space
    [InlineData("\u00A0")] // no-break space
    [InlineData("\u000A")] // line feed
    [InlineData("\u000D")] // carriage return
    [InlineData("\uFEFF")] // zw no-break space
    [InlineData("\u2029")] // paragraph separator (a unicode whitespace)
    void White_spaces_should_be_ignore(string expression)
    {
        var tokens = TokenizeUsingParser(expression);

        tokens.Should().HaveCount(1);
        tokens.Should().Contain(Token.End(1));
    }

    [Theory]
    [InlineData("\0")]
    [InlineData("@")]
    [InlineData("#")]
    [InlineData("\\")]
    void Unknown_characters_should_parse_properly(string expression)
    {
        var tokens = TokenizeUsingParser(expression);

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.Unknown(expression, 0));
        tokens.Last().Should().Be(Token.End(1));
    }

    [Fact]
    void Null_literal_should_parse_properly()
    {
        var tokens = TokenizeUsingParser("null");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.NullLiteral(0));
        tokens.Last().Should().Be(Token.End(4));
    }

    [Fact]
    void Boolean_true_literal_should_parse_properly()
    {
        var tokens = TokenizeUsingParser("true");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.TrueLiteral(0));
        tokens.Last().Should().Be(Token.End(4));
    }

    [Fact]
    void Boolean_false_literal_should_parse_properly()
    {
        var tokens = TokenizeUsingParser("false");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.FalseLiteral(0));
        tokens.Last().Should().Be(Token.End(5));
    }

    [Theory]
    [InlineData("0XABf2")]
    [InlineData("0xAbf2")]
    [InlineData("0xA_b_f2")]
    [InlineData("0xAbf2n")]
    [InlineData("0xA_b_f2n")]
    [InlineData("0156")]
    [InlineData("0O146")]
    [InlineData("0o156")]
    [InlineData("0o1_5_6")]
    [InlineData("0o156n")]
    [InlineData("0o1_5_6n")]
    [InlineData("0B10")]
    [InlineData("0b001")]
    [InlineData("0b0_1_1")]
    [InlineData("0b110n")]
    [InlineData("0b1_1_0n")]
    [InlineData("0")]
    [InlineData("1050")]
    [InlineData("100.5")]
    [InlineData("10.")]
    [InlineData(".15")]
    [InlineData("1_000")]
    [InlineData("1.2_987")]
    [InlineData("1_200.2_987")]
    [InlineData("1_200E+2_2")]
    [InlineData("1_200.2_987e-2")]
    [InlineData("1_200n")]
    void Numeric_literal_should_parse_properly(string expression)
    {
        var tokens = TokenizeUsingParser(expression);

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.NumericLiteral(expression, 0));
        tokens.Last().Should().Be(Token.End(expression.Length));
    }

    [Theory]
    [InlineData("0xz", "z", 2)]
    [InlineData("0x_", "_", 2)]
    [InlineData("0xa__b", "_", 4)]
    [InlineData("0xa_", "", 4)]
    [InlineData("0xn", "n", 2)]
    [InlineData("0x_n", "_", 2)]
    [InlineData("0xa_n", "n", 4)]
    [InlineData("0oz", "z", 2)]
    [InlineData("0o_", "_", 2)]
    [InlineData("0o2__6", "_", 4)]
    [InlineData("0o5_", "", 4)]
    [InlineData("0on", "n", 2)]
    [InlineData("0o_n", "_", 2)]
    [InlineData("0o5_n", "n", 4)]
    void Invalid_Numeric_literal_should_throw_exception(string expression, string invalidTokenText, int invalidTokenPosition)
    {
        Action parse = () => TokenizeUsingParser(expression);

        parse.Should().ThrowExactly<InvalidTokenException>().And.Token.Should().BeEquivalentTo(new
        {
            Text = invalidTokenText,
            Position = invalidTokenPosition
        });
    }

    [Theory]
    [InlineData("\"double quoted string\"")]
    [InlineData("\"double quoted string with single quote ' char\"")]
    [InlineData("'single quoted string'")]
    [InlineData("'single quoted string with double quote \" char'")]
    [InlineData("'string with escape \\' char'")]
    [InlineData("'multiline string \\\n and it is second line'")]
    [InlineData("'multiline string \\\r and it is second line'")]
    [InlineData("'multiline string \\\r\n and it is second line'")]
    void String_literals_should_parse_properly(string expression)
    {
        var tokens = TokenizeUsingParser(expression);

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.StringLiteral(expression, 0));
        tokens.Last().Should().Be(Token.End(expression.Length));
    }

    [Theory]
    [InlineData("`multiline template string \n and it is second line`")]
    [InlineData("`string with escape backtick \\` char`")]
    [InlineData("`escaped substitution \\${1+2} char`")]
    [InlineData("`escaped substitution $\\{1+2} char`")]
    void Template_literal_should_parse_properly(string expression)
    {
        var tokens = TokenizeUsingParser(expression);

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.TemplateLiteral(expression, 0));
        tokens.Last().Should().Be(Token.End(expression.Length));
    }

    [Fact]
    void Template_literal_with_substitutions_should_parse_properly()
    {
        var tokens = TokenizeUsingParser("`<:: 1 + 2 = ${1+2} | 1 - 2 = ${1-2} ::>`");

        tokens.Should().HaveCount(10);
        tokens.ElementAt(0).Should().Be(Token.TemplateHead("`<:: 1 + 2 = ${", 0));
        tokens.ElementAt(1).Should().Be(Token.NumericLiteral("1", 15));
        tokens.ElementAt(2).Should().Be(Token.Plus(16));
        tokens.ElementAt(3).Should().Be(Token.NumericLiteral("2", 17));
        tokens.ElementAt(4).Should().Be(Token.TemplateMiddle("} | 1 - 2 = ${", 18));
        tokens.ElementAt(5).Should().Be(Token.NumericLiteral("1", 32));
        tokens.ElementAt(6).Should().Be(Token.Minus(33));
        tokens.ElementAt(7).Should().Be(Token.NumericLiteral("2", 34));
        tokens.ElementAt(8).Should().Be(Token.TemplateTail("} ::>`", 35));
        tokens.Last().Should().Be(Token.End(41));
    }

    [Theory]
    [InlineData("// single line comment")]
    [InlineData("// single line comment*/x")]
    [InlineData("/* block comment */")]
    [InlineData("/* multiline block comment \n it is second line */")]
    void Comments_should_parse_properly(string expression)
    {
        var tokens = TokenizeUsingParser(expression);

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.Comment(expression, 0));
        tokens.Last().Should().Be(Token.End(expression.Length));
    }

    [Theory]
    [InlineData("/a/")]
    [InlineData("/a/gi")]
    [InlineData("/\\/a+/gi")]
    [InlineData("/^\\s*\\d+\\s*/gi")]
    void Regular_expression_literal_should_parse_properly(string expression)
    {
        var tokens = TokenizeUsingParser(expression);

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.RegExpLiteral(expression, 0));
        tokens.Last().Should().Be(Token.End(expression.Length));
    }

    [Theory]
    [InlineData("/", "", 1)]
    [InlineData("/\n/", "\n", 1)]
    void Invalid_regular_expression_literal_parse_should_throw_exception(string expression, string tokenText, int tokenPosition)
    {
        Action parse = () => TokenizeUsingParser(expression);

        parse.Should().ThrowExactly<InvalidTokenException>().And.Token.Should().BeEquivalentTo(new
        {
            Text = tokenText,
            Position = tokenPosition
        });
    }

    [Fact]
    void Identifier_should_parse_properly()
    {
        var tokens = TokenizeUsingParser("$name");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.Identifier("$name", 0));
        tokens.Last().Should().Be(Token.End(5));
    }

    [Fact]
    void Plus_sign_should_parse_properly()
    {
        var tokens = TokenizeUsingParser("+");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.Plus(0));
        tokens.Last().Should().Be(Token.End(1));
    }

    [Fact]
    void Minus_sign_should_parse_properly()
    {
        var tokens = TokenizeUsingParser("-");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.Minus(0));
        tokens.Last().Should().Be(Token.End(1));
    }

    [Fact]
    void Asterisk_sign_should_parse_properly()
    {
        var tokens = TokenizeUsingParser("*");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.Asterisk(0));
        tokens.Last().Should().Be(Token.End(1));
    }

    [Fact]
    void Double_asterisk_sign_should_parse_properly()
    {
        var tokens = TokenizeUsingParser("**");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.DoubleAsterisk(0));
        tokens.Last().Should().Be(Token.End(2));
    }

    [Fact]
    void Double_asterisk_equal_sign_should_parse_properly()
    {
        var tokens = TokenizeUsingParser("**=");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.DoubleAsteriskEqual(0));
        tokens.Last().Should().Be(Token.End(3));
    }

    [Fact]
    void Slash_sign_should_parse_properly()
    {
        var tokens = TokenizeUsingParser("1/2");

        tokens.Should().HaveCount(4);
        tokens.First().Should().Be(Token.NumericLiteral("1", 0));
        tokens.ElementAt(1).Should().Be(Token.Slash(1));
        tokens.ElementAt(2).Should().Be(Token.NumericLiteral("2", 2));
        tokens.Last().Should().Be(Token.End(3));
    }

    [Fact]
    void Percent_sign_should_parse_properly()
    {
        var tokens = TokenizeUsingParser("%");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.Percent(0));
        tokens.Last().Should().Be(Token.End(1));
    }

    [Fact]
    void Double_ampersand_sign_should_parse_properly()
    {
        var tokens = TokenizeUsingParser("&&");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.DoubleAmpersand(0));
        tokens.Last().Should().Be(Token.End(2));
    }

    [Fact]
    void Ampersand_sign_should_parse_properly()
    {
        var tokens = TokenizeUsingParser("&");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.Ampersand(0));
        tokens.Last().Should().Be(Token.End(1));
    }

    [Fact]
    void Ampersand_equal_sign_should_parse_properly()
    {
        var tokens = TokenizeUsingParser("&=");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.AmpersandEqual(0));
        tokens.Last().Should().Be(Token.End(2));
    }

    [Fact]
    void Double_ampersand_equal_sign_should_parse_properly()
    {
        var tokens = TokenizeUsingParser("&&=");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.DoubleAmpersandEqual(0));
        tokens.Last().Should().Be(Token.End(3));
    }

    [Fact]
    void Double_bar_sign_should_parse_properly()
    {
        var tokens = TokenizeUsingParser("||");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.DoubleBar(0));
        tokens.Last().Should().Be(Token.End(2));
    }

    [Fact]
    void Bar_equal_sign_should_parse_properly()
    {
        var tokens = TokenizeUsingParser("|=");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.BarEqual(0));
        tokens.Last().Should().Be(Token.End(2));
    }

    [Fact]
    void Double_bar_equal_sign_should_parse_properly()
    {
        var tokens = TokenizeUsingParser("||=");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.DoubleBarEqual(0));
        tokens.Last().Should().Be(Token.End(3));
    }

    [Fact]
    void Bar_sign_should_parse_properly()
    {
        var tokens = TokenizeUsingParser("|");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.Bar(0));
        tokens.Last().Should().Be(Token.End(1));
    }

    [Fact]
    void Exclamation_double_equal_sign_should_parse_properly()
    {
        var tokens = TokenizeUsingParser("!==");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.ExclamationDoubleEqual(0));
        tokens.Last().Should().Be(Token.End(3));
    }

    [Fact]
    void Exclamation_equal_sign_should_parse_properly()
    {
        var tokens = TokenizeUsingParser("!=");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.ExclamationEqual(0));
        tokens.Last().Should().Be(Token.End(2));
    }

    [Fact]
    void Exclamation_sign_should_parse_properly()
    {
        var tokens = TokenizeUsingParser("!");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.Exclamation(0));
        tokens.Last().Should().Be(Token.End(1));
    }

    [Fact]
    void Triple_equal_sign_should_parse_properly()
    {
        var tokens = TokenizeUsingParser("===");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.TripleEqual(0));
        tokens.Last().Should().Be(Token.End(3));
    }

    [Fact]
    void Double_equal_sign_should_parse_properly()
    {
        var tokens = TokenizeUsingParser("==");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.DoubleEqual(0));
        tokens.Last().Should().Be(Token.End(2));
    }

    [Fact]
    void Double_plus_sign_should_parse_properly()
    {
        var tokens = TokenizeUsingParser("++");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.DoublePlus(0));
        tokens.Last().Should().Be(Token.End(2));
    }

    [Fact]
    void Double_minus_sign_should_parse_properly()
    {
        var tokens = TokenizeUsingParser("--");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.DoubleMinus(0));
        tokens.Last().Should().Be(Token.End(2));
    }

    [Fact]
    void Equal_greater_than_sign_should_parse_properly()
    {
        var tokens = TokenizeUsingParser("=>");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.EqualGreaterThan(0));
        tokens.Last().Should().Be(Token.End(2));
    }

    [Fact]
    void Equal_sign_should_parse_properly()
    {
        var tokens = TokenizeUsingParser("=");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.Equal(0));
        tokens.Last().Should().Be(Token.End(1));
    }

    [Fact]
    void Plus_equal_sign_should_parse_properly()
    {
        var tokens = TokenizeUsingParser("+=");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.PlusEqual(0));
        tokens.Last().Should().Be(Token.End(2));
    }

    [Fact]
    void Minus_equal_sign_should_parse_properly()
    {
        var tokens = TokenizeUsingParser("-=");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.MinusEqual(0));
        tokens.Last().Should().Be(Token.End(2));
    }

    [Fact]
    void Asterisk_equal_sign_should_parse_properly()
    {
        var tokens = TokenizeUsingParser("*=");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.AsteriskEqual(0));
        tokens.Last().Should().Be(Token.End(2));
    }

    [Fact]
    void Slash_equal_sign_should_parse_properly()
    {
        var tokens = TokenizeUsingParser("1/=2");

        tokens.Should().HaveCount(4);
        tokens.First().Should().Be(Token.NumericLiteral("1", 0));
        tokens.ElementAt(1).Should().Be(Token.SlashEqual(1));
        tokens.ElementAt(2).Should().Be(Token.NumericLiteral("2", 3));
        tokens.Last().Should().Be(Token.End(4));
    }

    [Fact]
    void Percent_equal_sign_should_parse_properly()
    {
        var tokens = TokenizeUsingParser("%=");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.PercentEqual(0));
        tokens.Last().Should().Be(Token.End(2));
    }

    [Fact]
    void Double_question_sign_should_parse_properly()
    {
        var tokens = TokenizeUsingParser("??");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.DoubleQuestion(0));
        tokens.Last().Should().Be(Token.End(2));
    }

    [Fact]
    void Double_question_equal_sign_should_parse_properly()
    {
        var tokens = TokenizeUsingParser("??=");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.DoubleQuestionEqual(0));
        tokens.Last().Should().Be(Token.End(3));
    }

    [Fact]
    void Question_dot_sign_should_parse_properly()
    {
        var tokens = TokenizeUsingParser("?.");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.QuestionDot(0));
        tokens.Last().Should().Be(Token.End(2));
    }

    [Fact]
    void Question_sign_should_parse_properly()
    {
        var tokens = TokenizeUsingParser("?");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.Question(0));
        tokens.Last().Should().Be(Token.End(1));
    }

    [Fact]
    void Double_less_than_sign_should_parse_properly()
    {
        var tokens = TokenizeUsingParser("<<");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.DoubleLessThan(0));
        tokens.Last().Should().Be(Token.End(2));
    }

    [Fact]
    void Double_less_than_equal_sign_should_parse_properly()
    {
        var tokens = TokenizeUsingParser("<<=");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.DoubleLessThanEqual(0));
        tokens.Last().Should().Be(Token.End(3));
    }

    [Fact]
    void Double_greater_than_equal_sign_should_parse_properly()
    {
        var tokens = TokenizeUsingParser(">>=");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.DoubleGreaterThanEqual(0));
        tokens.Last().Should().Be(Token.End(3));
    }

    [Fact]
    void Triple_greater_than_equal_sign_should_parse_properly()
    {
        var tokens = TokenizeUsingParser(">>>=");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.TripleGreaterThanEqual(0));
        tokens.Last().Should().Be(Token.End(4));
    }

    [Fact]
    void Less_than_equal_sign_should_parse_properly()
    {
        var tokens = TokenizeUsingParser("<=");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.LessThanEqual(0));
        tokens.Last().Should().Be(Token.End(2));
    }

    [Fact]
    void Less_than_sign_should_parse_properly()
    {
        var tokens = TokenizeUsingParser("<");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.LessThan(0));
        tokens.Last().Should().Be(Token.End(1));
    }

    [Fact]
    void Triple_greater_than_sign_should_parse_properly()
    {
        var tokens = TokenizeUsingParser(">>>");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.TripleGreaterThan(0));
        tokens.Last().Should().Be(Token.End(3));
    }

    [Fact]
    void Double_greater_than_sign_should_parse_properly()
    {
        var tokens = TokenizeUsingParser(">>");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.DoubleGreaterThan(0));
        tokens.Last().Should().Be(Token.End(2));
    }

    [Fact]
    void Greater_than_sign_should_parse_properly()
    {
        var tokens = TokenizeUsingParser(">");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.GreaterThan(0));
        tokens.Last().Should().Be(Token.End(1));
    }

    [Fact]
    void Greater_than_equal_sign_should_parse_properly()
    {
        var tokens = TokenizeUsingParser(">=");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.GreaterThanEqual(0));
        tokens.Last().Should().Be(Token.End(2));
    }

    [Fact]
    void Parenthesis_signs_should_parse_properly()
    {
        var openBracketTokens = TokenizeUsingParser("(");

        openBracketTokens.Should().HaveCount(2);
        openBracketTokens.First().Should().Be(Token.OpenParen(0));
        openBracketTokens.Last().Should().Be(Token.End(1));

        var closeBracketTokens = TokenizeUsingParser(")");

        closeBracketTokens.Should().HaveCount(2);
        closeBracketTokens.First().Should().Be(Token.CloseParen(0));
        closeBracketTokens.Last().Should().Be(Token.End(1));
    }

    [Fact]
    void Bracket_signs_should_parse_properly()
    {
        var openBracketTokens = TokenizeUsingParser("[");

        openBracketTokens.Should().HaveCount(2);
        openBracketTokens.First().Should().Be(Token.OpenBracket(0));
        openBracketTokens.Last().Should().Be(Token.End(1));

        var closeBracketTokens = TokenizeUsingParser("]");

        closeBracketTokens.Should().HaveCount(2);
        closeBracketTokens.First().Should().Be(Token.CloseBracket(0));
        closeBracketTokens.Last().Should().Be(Token.End(1));
    }

    [Fact]
    void Braces_signs_should_parse_properly()
    {
        var openBraceTokens = TokenizeUsingParser("{");

        openBraceTokens.Should().HaveCount(2);
        openBraceTokens.First().Should().Be(Token.OpenBrace(0));
        openBraceTokens.Last().Should().Be(Token.End(1));

        var closeBraceTokens = TokenizeUsingParser("}");

        closeBraceTokens.Should().HaveCount(2);
        closeBraceTokens.First().Should().Be(Token.CloseBrace(0));
        closeBraceTokens.Last().Should().Be(Token.End(1));
    }

    [Fact]
    void Dot_sign_should_parse_properly()
    {
        var tokens = TokenizeUsingParser(".");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.Dot(0));
        tokens.Last().Should().Be(Token.End(1));
    }

    [Fact]
    void Comma_sign_should_parse_properly()
    {
        var tokens = TokenizeUsingParser(",");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.Comma(0));
        tokens.Last().Should().Be(Token.End(1));
    }

    [Fact]
    void Tilde_sign_should_parse_properly()
    {
        var tokens = TokenizeUsingParser("~");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.Tilde(0));
        tokens.Last().Should().Be(Token.End(1));
    }

    [Fact]
    void Carat_sign_should_parse_properly()
    {
        var tokens = TokenizeUsingParser("^");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.Carat(0));
        tokens.Last().Should().Be(Token.End(1));
    }

    [Fact]
    void Carat_equal_sign_should_parse_properly()
    {
        var tokens = TokenizeUsingParser("^=");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.CaratEqual(0));
        tokens.Last().Should().Be(Token.End(2));
    }

    [Fact]
    void Colon_sign_should_parse_properly()
    {
        var tokens = TokenizeUsingParser(":");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.Colon(0));
        tokens.Last().Should().Be(Token.End(1));
    }

    [Fact]
    void Semicolon_sign_should_parse_properly()
    {
        var tokens = TokenizeUsingParser(";");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.Semicolon(0));
        tokens.Last().Should().Be(Token.End(1));
    }

    [Fact]
    void Simple_mathematic_expression_should_parse_properly()
    {
        var tokens = TokenizeUsingParser("-2 + 5");

        tokens.Should().HaveCount(5);
        tokens.First().Should().Be(Token.Minus(0));
        tokens.ElementAt(1).Should().Be(Token.NumericLiteral("2", 1));
        tokens.ElementAt(2).Should().Be(Token.Plus(3));
        tokens.ElementAt(3).Should().Be(Token.NumericLiteral("5", 5));
        tokens.Last().Should().Be(Token.End(6));
    }

    [Fact]
    void New_expression_to_string_should_parse_properly()
    {
        var tokens = TokenizeUsingParser("new Date()");

        tokens.Should().HaveCount(5);
        tokens.First().ToText().Should().Be("new ");
        tokens.First().ToString().Should().Be("new ");
    }

    [Fact]
    void Triple_dot_sign_should_parse_properly()
    {
        var tokens = TokenizeUsingParser("...");

        tokens.Should().HaveCount(2);
        tokens.First().Should().Be(Token.TripleDot(0));
        tokens.Last().Should().Be(Token.End(3));
    }

    [Theory]
    [InlineData("\"", "", 1)]
    [InlineData("'", "", 1)]
    [InlineData("`", "", 1)]
    [InlineData("\"a", "", 2)]
    [InlineData("'a", "", 2)]
    [InlineData("`a", "", 2)]
    [InlineData("'a\\'", "", 4)]
    [InlineData("/*", "", 2)]
    [InlineData("/*abc", "", 5)]
    void Invalid_expression_parse_should_throw_exception(string expression, string invalidTokenText, int invalidTokenPosition)
    {
        Action parse = () => TokenizeUsingParser(expression);

        parse.Should().ThrowExactly<InvalidTokenException>().And.Token.Should().BeEquivalentTo(new
        {
            Text = invalidTokenText,
            Position = invalidTokenPosition
        });
    }

    IReadOnlyList<Token> TokenizeUsingParser(string text)
    {
        var tokens = new List<Token>();
        var parser = new TextParser(text);
        while (true)
        {
            var token = parser.ReadToken();
            tokens.Add(token);
            if (token.Type == TokenType.End) break;
        }
        return tokens;
    }
}
