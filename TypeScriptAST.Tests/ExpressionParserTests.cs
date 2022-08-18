using System.Numerics;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using TypeScriptAST.Declarations;
using TypeScriptAST.Expressions;
using Xunit;
using TemplateElement = TypeScriptAST.Expressions.TemplateLiteral.TemplateElement;
using Type = TypeScriptAST.Declarations.Types.Type;

namespace TypeScriptAST.Tests;

public class ExpressionParserTests
{
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData("\r")]
    [InlineData("\n")]
    void Empty_or_white_space_only_expressions_parse_should_throw_exception(string expression)
    {
        var parse = () => Parse(expression);

        parse.Should().ThrowExactly<InvalidTokenException>();
    }

    [Theory]
    [InlineData("// single line comment")]
    [InlineData("/* block comment */")]
    [InlineData("/* multiline block comment \n it is second line */")]
    void Comment_only_expressions_parse_should_throw_exception(string expression)
    {
        var parse = () => Parse(expression);

        parse.Should().ThrowExactly<InvalidTokenException>();
    }

    [Theory]
    [InlineData("\0")]
    [InlineData("@")]
    [InlineData("#")]
    [InlineData("\\")]
    void Unknown_characters_should_throw_exception(string expression)
    {
        var read = () => Parse(expression);

        read.Should().ThrowExactly<InvalidTokenException>();
    }

    [Theory]
    [InlineData("0XABf2", 0XABf2L)]
    [InlineData("0xAbf2", 0xAbf2L)]
    [InlineData("0xA_b_f2", 0xA_b_f2L)]
    [InlineData("0156", 110L)]
    [InlineData("0O146", 102L)]
    [InlineData("0o156", 110L)]
    [InlineData("0o1_5_6", 110L)]
    [InlineData("0B10", 0B10L)]
    [InlineData("0b001", 0b001L)]
    [InlineData("0b0_1_1", 0b0_1_1L)]
    [InlineData("0", 0L)]
    [InlineData("081", 81L)]
    [InlineData("1050", 1050L)]
    [InlineData("100.5", 100.5D)]
    [InlineData("10.", 10.0D)]
    [InlineData(".15", 0.15D)]
    [InlineData("1_000", 1_000L)]
    [InlineData("1.2_987", 1.2_987D)]
    [InlineData("1_200.2_987", 1_200.2_987D)]
    [InlineData("1_200E+2_2", 1_200E+2_2D)]
    [InlineData("1_200.2_987e-2", 1_200.2_987e-2D)]
    void Numeric_literal_should_parse_properly(string expression, object expectedValue)
    {
        var actualExpression = Parse(expression);

        actualExpression.Should().BeOfType<Literal>();
        actualExpression.As<Literal>().Should().BeEquivalentTo(new
        {
            Value = expectedValue,
            Type = Type.Number
        });
    }

    [Theory]
    [InlineData("0xAbf2n", 0xAbf2)]
    [InlineData("0xA_b_f2n", 0xA_b_f2)]
    [InlineData("0o156n", 110)]
    [InlineData("0o1_5_6n", 110)]
    [InlineData("0b110n", 0b110)]
    [InlineData("0b1_1_0n", 0b1_1_0)]
    [InlineData("1_200n", 1_200)]
    void BigInteger_literal_should_parse_properly(string expression, BigInteger expectedValue)
    {
        var actualExpression = Parse(expression);

        actualExpression.Should().BeOfType<Literal>();
        actualExpression.As<Literal>().Should().BeEquivalentTo(new
        {
            Value = expectedValue,
            Type = Type.BigInt
        });
    }

    [Theory]
    [InlineData("\"")]
    [InlineData("\"a")]
    [InlineData("'a")]
    [InlineData("`a")]
    [InlineData("'a\\")]
    [InlineData("'\\x")]
    [InlineData("'\\xy")]
    [InlineData("'\\x0")]
    [InlineData("'\\uy")]
    [InlineData("'\\u0")]
    [InlineData("'\\u00")]
    [InlineData("'\\u0ff")]
    [InlineData("'\\u{")]
    [InlineData("'\\u{y")]
    [InlineData("'\\u{y}")]
    void Invalid_string_format_should_throw_exception(string expression)
    {
        var parse = () => Parse(expression);

        parse.Should().ThrowExactly<InvalidTokenException>();
    }

    [Theory]
    [InlineData("\"double quoted string\"", "double quoted string")]
    [InlineData("\"double quoted string with single quote ' char\"", "double quoted string with single quote ' char")]
    [InlineData("'single quoted string'", "single quoted string")]
    [InlineData("'single quoted string with double quote \" char'", "single quoted string with double quote \" char")]
    [InlineData("'string with escape \\' char'", "string with escape ' char")]
    [InlineData("'multiline string \\\n and it is second line'", "multiline string  and it is second line")]
    [InlineData("'multiline string \\\r and it is second line'", "multiline string  and it is second line")]
    [InlineData("'multiline string \\\r\n and it is second line'", "multiline string  and it is second line")]
    [InlineData("'\\0'", "\0")]
    [InlineData("'\\xff'", "\xff")]
    [InlineData("'\\u00ff'", "\u00ff")]
    [InlineData("'\\u{10FF}'", "\u10ff")]
    [InlineData("'\\t\\b\\f\\v'", "\t\b\f\v")]
    [InlineData("'\\7\\2'", "\u0007\u0002")]
    [InlineData("'\\19'", "\u00019")]
    [InlineData("'\\379'", "\u001F9")]
    [InlineData("'\\375'", "\u00FD")]
    [InlineData("'\\45'", "\u0025")]
    void String_literal_should_parse_properly(string expression, string expectedValue)
    {
        var actualExpression = Parse(expression);

        actualExpression.Should().BeOfType<Literal>();
        actualExpression.As<Literal>().Should().BeEquivalentTo(new
        {
            Value = expectedValue,
            Type = Type.String
        });
    }

    [Theory]
    [InlineData("`template string \n and its second line`", "template string \n and its second line")]
    [InlineData("`string with escape backtick \\` char`", "string with escape backtick ` char")]
    [InlineData("`\\0`", "\0")]
    [InlineData("`escaped substitution \\${1+2} char`", "escaped substitution ${1+2} char")]
    [InlineData("`escaped substitution $\\{1+2} char`", "escaped substitution ${1+2} char")]
    void Template_literal_should_parse_properly(string expression, string expectedValue)
    {
        var actualExpression = Parse(expression);

        actualExpression.Should().BeOfType<TemplateLiteral>();
        actualExpression.As<TemplateLiteral>().Elements.Should().HaveCount(1);
        actualExpression.As<TemplateLiteral>().Elements.Should().BeEquivalentTo(new TemplateElement[]
        {
            expectedValue
        });
    }

    [Fact]
    void Template_literal_with_substitutions_should_parse_properly()
    {
        var actualExpression = Parse("`(1+2=${1+2},1-2=${1-2})`");

        actualExpression.Should().BeOfType<TemplateLiteral>();
        actualExpression.As<TemplateLiteral>().Elements.Should().HaveCount(5);
        actualExpression.As<TemplateLiteral>().Elements[0].Value.Should().Be("(1+2=");
        actualExpression.As<TemplateLiteral>().Elements[1].Expression.Should().BeOfType<Add>();
        actualExpression.As<TemplateLiteral>().Elements[2].Value.Should().Be(",1-2=");
        actualExpression.As<TemplateLiteral>().Elements[3].Expression.Should().BeOfType<Subtract>();
        actualExpression.As<TemplateLiteral>().Elements[4].Value.Should().Be(")");
    }

    [Fact]
    void Nested_template_literal_with_substitutions_should_parse_properly()
    {
        var actualExpression = Parse("`1+${`2=${3}`}`");

        actualExpression.Should().BeOfType<TemplateLiteral>();
        actualExpression.As<TemplateLiteral>().Elements.Should().HaveCount(3);
        actualExpression.As<TemplateLiteral>().Elements[0].Value.Should().Be("1+");
        actualExpression.As<TemplateLiteral>().Elements[1].Expression.Should().BeOfType<TemplateLiteral>()
            .Which.Elements.Should().HaveCount(3);
        actualExpression.As<TemplateLiteral>().Elements[2].Value.Should().BeEmpty();
    }

    [Theory]
    [InlineData("/a/", "a", "")]
    [InlineData("/a/gi", "a", "gi")]
    [InlineData("/\\/a+/gi", "/a+", "gi")]
    [InlineData("/^\\s*\\d+\\s*/gi", "^\\s*\\d+\\s*", "gi")]
    void Regular_expression_literal_should_parse_properly(string expression, string expectedExpression, string expectedFlags)
    {
        var actualExpression = Parse(expression);

        actualExpression.Should().BeOfType<RegularExpression>();
        actualExpression.As<RegularExpression>().Should().BeEquivalentTo(new
        {
            Expression = expectedExpression,
            Flags = expectedFlags
        });
    }

    [Fact]
    void Add_and_subtract_operators_should_parse_properly()
    {
        var actualExpression = Parse("2 + 9 - 8");

        actualExpression.Should().BeOfType<Subtract>();
        actualExpression.As<Subtract>().Left.Should().BeOfType<Add>();
        actualExpression.As<Subtract>().Right.Should().BeOfType<Literal>();
    }

    [Fact]
    void Multiply_and_divide_operators_should_parse_properly()
    {
        var actualExpression = Parse("2 * 9 / 3");

        actualExpression.Should().BeOfType<Divide>();
        actualExpression.As<Divide>().Left.Should().BeOfType<Multiply>();
        actualExpression.As<Divide>().Right.Should().BeOfType<Literal>();
    }

    [Fact]
    void Negative_and_positive_operators_should_parse_properly()
    {
        var actualExpression = Parse("-2 + +5");

        actualExpression.Should().BeOfType<Add>();
        actualExpression.As<Add>().Left.Should().BeOfType<Negate>();
        actualExpression.As<Add>().Right.Should().BeOfType<UnaryPlus>();
    }

    [Fact]
    void Operator_precedence_should_parse_properly()
    {
        var actualExpression = Parse("2 + 5 * 5");

        actualExpression.Should().BeOfType<Add>();
        actualExpression.As<Add>().Left.Should().BeOfType<Literal>();
        actualExpression.As<Add>().Right.Should().BeOfType<Multiply>();
    }

    [Fact]
    void Group_operator_precedence_should_parse_properly()
    {
        var actualExpression = Parse("(2 + 5) * 5");

        actualExpression.Should().BeOfType<Multiply>();
        actualExpression.As<Multiply>().Left.Should().BeOfType<Add>();
        actualExpression.As<Multiply>().Right.Should().BeOfType<Literal>();
    }

    [Fact]
    void Simple_mathematical_expressions_should_evaluate_properly()
    {
        new Evaluator().Evaluate(Parse("20-(8+1-3)*2")).Should().Be(8);
        new Evaluator().Evaluate(Parse("(3-(5-(8)-(2+(9-(0-(8-(2))))-(4))-(4)))")).Should().Be(23);
        new Evaluator().Evaluate(Parse("3 - (1 + 2 - 2)")).Should().Be(2);
        new Evaluator().Evaluate(Parse(" 2-(1 + 2) ")).Should().Be(-1);
        new Evaluator().Evaluate(Parse(" 2-1 + 2 ")).Should().Be(3);
        new Evaluator().Evaluate(Parse("(1+(4+5+2)-3)+(6+8)")).Should().Be(23);
        new Evaluator().Evaluate(Parse("(1+5)-(2+0)")).Should().Be(4);
        new Evaluator().Evaluate(Parse("(1+5)-2")).Should().Be(4);
        new Evaluator().Evaluate(Parse("(1+5-2)")).Should().Be(4);
        new Evaluator().Evaluate(Parse("(1+2)")).Should().Be(3);
    }

    [Fact]
    void BuiltIn_values_should_parse_properly()
    {
        var actualTrueExpression = Parse("true");
        var actualFalseExpression = Parse("false");
        var actualNullExpression = Parse("null");
        var actualNaNExpression = Parse("NaN");

        actualTrueExpression.Should().BeOfType<Literal>().Which.Should().BeEquivalentTo(new
        {
            Value = true,
            Type = Type.Boolean
        });
        actualFalseExpression.Should().BeOfType<Literal>().Which.Should().BeEquivalentTo(new
        {
            Value = false,
            Type = Type.Boolean
        });
        actualNullExpression.Should().BeOfType<Literal>().Which.Should().BeEquivalentTo(new
        {
            Value = (object)null,
            Type = Type.Any
        });
        actualNaNExpression.Should().BeOfType<Literal>().Which.Should().BeEquivalentTo(new
        {
            Value = Double.NaN,
            Type = Type.Number
        });
    }

    [Fact]
    void Undefined_keyword_should_parse_properly()
    {
        var actualExpression = Parse("undefined");

        actualExpression.Should().BeOfType<Literal>();
        actualExpression.As<Literal>().Should().BeEquivalentTo(new
        {
            Value = Undefined.Value,
            Type = Type.Any
        });
    }

    [Fact]
    void Remainder_operator_should_parse_properly()
    {
        var actualExpression = Parse("5 % 2");

        actualExpression.Should().BeOfType<Remainder>();
        actualExpression.As<Remainder>().Left.Should().BeOfType<Literal>();
        actualExpression.As<Remainder>().Right.Should().BeOfType<Literal>();
    }

    [Fact]
    void Exponentiation_operator_should_parse_properly()
    {
        var actualExpression = Parse("5 ** 2");

        actualExpression.Should().BeOfType<Exponent>();
        actualExpression.As<Exponent>().Left.Should().BeOfType<Literal>();
        actualExpression.As<Exponent>().Right.Should().BeOfType<Literal>();
    }

    [Fact]
    void Logical_not_operator_should_parse_properly()
    {
        var actualExpression = Parse("! true");

        actualExpression.Should().BeOfType<LogicalNot>();
        actualExpression.As<LogicalNot>().Operand.Should().BeOfType<Literal>();
    }

    [Fact]
    void Logical_and_operator_should_parse_properly()
    {
        var actualExpression = Parse("false && true");

        actualExpression.Should().BeOfType<LogicalAnd>();
        actualExpression.As<LogicalAnd>().Left.Should().BeOfType<Literal>();
        actualExpression.As<LogicalAnd>().Right.Should().BeOfType<Literal>();
    }

    [Fact]
    void Logical_or_operator_should_parse_properly()
    {
        var actualExpression = Parse("false || true");

        actualExpression.Should().BeOfType<LogicalOr>();
        actualExpression.As<LogicalOr>().Left.Should().BeOfType<Literal>();
        actualExpression.As<LogicalOr>().Right.Should().BeOfType<Literal>();
    }

    [Fact]
    void Bitwise_not_operator_should_parse_properly()
    {
        var actualExpression = Parse("~ 10");

        actualExpression.Should().BeOfType<BitwiseNot>();
        actualExpression.As<BitwiseNot>().Operand.Should().BeOfType<Literal>();
    }

    [Fact]
    void Bitwise_and_operator_should_parse_properly()
    {
        var actualExpression = Parse("10 & 5");

        actualExpression.Should().BeOfType<BitwiseAnd>();
        actualExpression.As<BitwiseAnd>().Left.Should().BeOfType<Literal>();
        actualExpression.As<BitwiseAnd>().Right.Should().BeOfType<Literal>();
    }

    [Fact]
    void Bitwise_or_operator_should_parse_properly()
    {
        var actualExpression = Parse("10 | 5");

        actualExpression.Should().BeOfType<BitwiseOr>();
        actualExpression.As<BitwiseOr>().Left.Should().BeOfType<Literal>();
        actualExpression.As<BitwiseOr>().Right.Should().BeOfType<Literal>();
    }

    [Fact]
    void Bitwise_xor_operator_should_parse_properly()
    {
        var actualExpression = Parse("10 ^ 5");

        actualExpression.Should().BeOfType<BitwiseXor>();
        actualExpression.As<BitwiseXor>().Left.Should().BeOfType<Literal>();
        actualExpression.As<BitwiseXor>().Right.Should().BeOfType<Literal>();
    }

    [Fact]
    void Bitwise_left_shift_operator_should_parse_properly()
    {
        var actualExpression = Parse("10 << 2");

        actualExpression.Should().BeOfType<BitwiseLeftShift>();
        actualExpression.As<BitwiseLeftShift>().Left.Should().BeOfType<Literal>();
        actualExpression.As<BitwiseLeftShift>().Right.Should().BeOfType<Literal>();
    }

    [Fact]
    void Bitwise_right_shift_operator_should_parse_properly()
    {
        var actualExpression = Parse("10 >> 2");

        actualExpression.Should().BeOfType<BitwiseRightShift>();
        actualExpression.As<BitwiseRightShift>().Left.Should().BeOfType<Literal>();
        actualExpression.As<BitwiseRightShift>().Right.Should().BeOfType<Literal>();
    }

    [Fact]
    void Bitwise_unsigned_right_shift_operator_should_parse_properly()
    {
        var actualExpression = Parse("10 >>> 2");

        actualExpression.Should().BeOfType<BitwiseUnsignedRightShift>();
        actualExpression.As<BitwiseUnsignedRightShift>().Left.Should().BeOfType<Literal>();
        actualExpression.As<BitwiseUnsignedRightShift>().Right.Should().BeOfType<Literal>();
    }

    [Fact]
    void Less_than_operator_should_parse_properly()
    {
        var actualExpression = Parse("10 < 50");

        actualExpression.Should().BeOfType<Expressions.LessThan>();
        actualExpression.As<Expressions.LessThan>().Left.Should().BeOfType<Literal>();
        actualExpression.As<Expressions.LessThan>().Right.Should().BeOfType<Literal>();
    }

    [Fact]
    void Less_than_or_equal_operator_should_parse_properly()
    {
        var actualExpression = Parse("10 <= 50");

        actualExpression.Should().BeOfType<LessThanOrEqual>();
        actualExpression.As<LessThanOrEqual>().Left.Should().BeOfType<Literal>();
        actualExpression.As<LessThanOrEqual>().Right.Should().BeOfType<Literal>();
    }

    [Fact]
    void Greater_than_operator_should_parse_properly()
    {
        var actualExpression = Parse("10 > 2");

        actualExpression.Should().BeOfType<GreaterThan>();
        actualExpression.As<GreaterThan>().Left.Should().BeOfType<Literal>();
        actualExpression.As<GreaterThan>().Right.Should().BeOfType<Literal>();
    }

    [Fact]
    void Greater_than_or_equal_operator_should_parse_properly()
    {
        var actualExpression = Parse("10 >= 5");

        actualExpression.Should().BeOfType<GreaterThanOrEqual>();
        actualExpression.As<GreaterThanOrEqual>().Left.Should().BeOfType<Literal>();
        actualExpression.As<GreaterThanOrEqual>().Right.Should().BeOfType<Literal>();
    }

    [Fact]
    void Equality_operator_should_parse_properly()
    {
        var actualExpression = Parse("10 == 50");

        actualExpression.Should().BeOfType<Equality>();
        actualExpression.As<Equality>().Left.Should().BeOfType<Literal>();
        actualExpression.As<Equality>().Right.Should().BeOfType<Literal>();
    }

    [Fact]
    void Inequality_operator_should_parse_properly()
    {
        var actualExpression = Parse("10 != 50");

        actualExpression.Should().BeOfType<Inequality>();
        actualExpression.As<Inequality>().Left.Should().BeOfType<Literal>();
        actualExpression.As<Inequality>().Right.Should().BeOfType<Literal>();
    }

    [Fact]
    void Strict_equality_operator_should_parse_properly()
    {
        var actualExpression = Parse("10 === 2");

        actualExpression.Should().BeOfType<StrictEquality>();
        actualExpression.As<StrictEquality>().Left.Should().BeOfType<Literal>();
        actualExpression.As<StrictEquality>().Right.Should().BeOfType<Literal>();
    }

    [Fact]
    void Strict_inequality_operator_should_parse_properly()
    {
        var actualExpression = Parse("10 !== 5");

        actualExpression.Should().BeOfType<StrictInequality>();
        actualExpression.As<StrictInequality>().Left.Should().BeOfType<Literal>();
        actualExpression.As<StrictInequality>().Right.Should().BeOfType<Literal>();
    }

    [Fact]
    void Nullish_coalescing_operator_should_parse_properly()
    {
        var actualExpression = Parse("0 ?? 5");

        actualExpression.Should().BeOfType<NullishCoalescing>();
        actualExpression.As<NullishCoalescing>().Left.Should().BeOfType<Literal>();
        actualExpression.As<NullishCoalescing>().Right.Should().BeOfType<Literal>();
    }

    [Fact]
    void Conditional_operator_should_parse_properly()
    {
        var actualExpression = Parse("true ? 1 : 5");

        actualExpression.Should().BeOfType<Conditional>();
        actualExpression.As<Conditional>().Condition.Should().BeOfType<Literal>();
        actualExpression.As<Conditional>().TrueExpression.Should().BeOfType<Literal>();
        actualExpression.As<Conditional>().FalseExpression.Should().BeOfType<Literal>();
    }

    [Fact]
    void InstanceOf_builtin_operator_should_parse_properly()
    {
        var actualExpression = Parse("null instanceof Number ?? null instanceof(Date)");

        actualExpression.As<NullishCoalescing>().Left.Should().BeOfType<InstanceOf>();
        actualExpression.As<NullishCoalescing>().Right.Should().BeOfType<InstanceOf>();
    }

    [Fact]
    void In_builtin_operator_should_parse_properly()
    {
        var actualExpression = Parse("null in [] ?? 5 in([1,2])");

        actualExpression.As<NullishCoalescing>().Left.Should().BeOfType<In>();
        actualExpression.As<NullishCoalescing>().Right.Should().BeOfType<In>();
    }

    [Fact]
    void As_builtin_operator_should_parse_properly()
    {
        var actualExpression = Parse("null as Number ?? null as (Number)");

        actualExpression.As<NullishCoalescing>().Left.Should().BeOfType<As>();
        actualExpression.As<NullishCoalescing>().Right.Should().BeOfType<As>();
    }

    [Fact]
    void TypeOf_builtin_operator_should_parse_properly()
    {
        var actualExpression = Parse("typeof 5 ?? typeof(null)");

        actualExpression.As<NullishCoalescing>().Left.Should().BeOfType<TypeOf>();
        actualExpression.As<NullishCoalescing>().Right.Should().BeOfType<TypeOf>();
    }

    [Fact]
    void Void_builtin_operator_should_parse_properly()
    {
        var actualExpression = Parse("void '\"' ?? void(0)");

        actualExpression.As<NullishCoalescing>().Left.Should().BeOfType<VoidOf>();
        actualExpression.As<NullishCoalescing>().Right.Should().BeOfType<VoidOf>();
    }

    [Theory]
    [InlineData("1 2", "2", 2)]
    [InlineData("[", "", 1)]
    [InlineData("]", "]", 0)]
    [InlineData("[1", "", 2)]
    [InlineData("[,", ",", 1)]
    [InlineData("[1,", "", 3)]
    [InlineData("+", "", 1)]
    [InlineData("-", "", 1)]
    [InlineData("1+", "", 2)]
    [InlineData("1-", "", 2)]
    [InlineData("*", "*", 0)]
    [InlineData("1*", "", 2)]
    [InlineData("*1", "*", 0)]
    [InlineData("/", "", 1)]
    [InlineData("1/", "", 2)]
    [InlineData("/1", "", 2)]
    [InlineData("%", "%", 0)]
    [InlineData("1%", "", 2)]
    [InlineData("%1", "%", 0)]
    [InlineData("**", "**", 0)]
    [InlineData("1**", "", 3)]
    [InlineData("**1", "**", 0)]
    [InlineData("!", "", 1)]
    [InlineData("&&", "&&", 0)]
    [InlineData("true &&", "", 7)]
    [InlineData("&& true", "&&", 0)]
    [InlineData("||", "||", 0)]
    [InlineData("true ||", "", 7)]
    [InlineData("|| true", "||", 0)]
    [InlineData("~", "", 1)]
    [InlineData("&", "&", 0)]
    [InlineData("1 &", "", 3)]
    [InlineData("& 1", "&", 0)]
    [InlineData("|", "|", 0)]
    [InlineData("1 |", "", 3)]
    [InlineData("| 1", "|", 0)]
    [InlineData("^", "^", 0)]
    [InlineData("1 ^", "", 3)]
    [InlineData("^ 1", "^", 0)]
    [InlineData("<<", "<<", 0)]
    [InlineData("1 <<", "", 4)]
    [InlineData("<< 1", "<<", 0)]
    [InlineData(">>", ">>", 0)]
    [InlineData("1 >>", "", 4)]
    [InlineData(">> 1", ">>", 0)]
    [InlineData(">>>", ">>>", 0)]
    [InlineData("1 >>>", "", 5)]
    [InlineData(">>> 1", ">>>", 0)]
    [InlineData("<", "<", 0)]
    [InlineData("1 <", "", 3)]
    [InlineData("< 1", "<", 0)]
    [InlineData("<=", "<=", 0)]
    [InlineData("1 <=", "", 4)]
    [InlineData("<= 1", "<=", 0)]
    [InlineData(">", ">", 0)]
    [InlineData("1 >", "", 3)]
    [InlineData("> 1", ">", 0)]
    [InlineData(">=", ">=", 0)]
    [InlineData("1 >=", "", 4)]
    [InlineData(">= 1", ">=", 0)]
    [InlineData("==", "==", 0)]
    [InlineData("1 ==", "", 4)]
    [InlineData("== 1", "==", 0)]
    [InlineData("!=", "!=", 0)]
    [InlineData("1 !=", "", 4)]
    [InlineData("!= 1", "!=", 0)]
    [InlineData("===", "===", 0)]
    [InlineData("1 ===", "", 5)]
    [InlineData("=== 1", "===", 0)]
    [InlineData("!==", "!==", 0)]
    [InlineData("1 !==", "", 5)]
    [InlineData("!== 1", "!==", 0)]
    [InlineData("??", "??", 0)]
    [InlineData("1 ??", "", 4)]
    [InlineData("?? 1", "??", 0)]
    [InlineData("?", "?", 0)]
    [InlineData("?:", "?", 0)]
    [InlineData("?1:3", "?", 0)]
    [InlineData("1?", "", 2)]
    [InlineData("1?2", "", 3)]
    [InlineData("1?:2", ":", 2)]
    [InlineData("1?:", ":", 2)]
    [InlineData("(", "", 1)]
    [InlineData("(1", "", 2)]
    [InlineData("(1,", ",", 2)]
    [InlineData(".", ".", 0)]
    [InlineData("''.", "", 3)]
    [InlineData(".length", ".", 0)]
    [InlineData(")", ")", 0)]
    [InlineData("NaN[", "", 4)]
    [InlineData("NaN[2", "", 5)]
    [InlineData("isNaN(", "", 6)]
    [InlineData("isNaN(,", ",", 6)]
    [InlineData("isNaN(,)", ",", 6)]
    [InlineData("instanceof", "instanceof", 0)]
    [InlineData("1 instanceof", "", 12)]
    [InlineData("instanceof number", "instanceof", 0)]
    [InlineData("as", "as", 0)]
    [InlineData("0 as", "", 4)]
    [InlineData("as number", "as", 0)]
    [InlineData("in", "in", 0)]
    [InlineData("1 in", "", 4)]
    [InlineData("in []", "in", 0)]
    [InlineData("typeof", "", 6)]
    [InlineData("void", "", 4)]
    [InlineData("new", "", 3)]
    [InlineData("new String", "", 10)]
    void Invalid_expression_parse_should_throw_exception(
        string expression,
        string invalidTokenText,
        int invalidTokenPosition)
    {
        Action parse = () => Parse(expression);

        parse.Should().ThrowExactly<InvalidTokenException>().And.Token.Should().BeEquivalentTo(new
        {
            Text = invalidTokenText,
            Position = invalidTokenPosition
        });
    }

    [Fact]
    void Array_should_parse_properly()
    {
        var actualExpression = Parse("[1,2,'a',5.5]");

        actualExpression.Should().BeOfType<Expressions.Array>();
        actualExpression.As<Expressions.Array>().Items.Should().HaveCount(4);
    }

    [Fact]
    void Identifier_should_parse_properly()
    {
        var module = new ModuleDef
        {
            Constants = { new ConstDeclaration { Name = "n", Type = Type.Number } }
        };

        var actualExpression = Parse("n", module);

        actualExpression.Should().BeOfType<Identifier>();
        actualExpression.As<Identifier>().Should().BeEquivalentTo(new
        {
            Name = "n",
            Type = Type.Number
        });
    }

    [Fact]
    void Enum_value_should_parse_by_index_properly()
    {
        var alphabet = Type.CreateEnum("Alphabet", "A", "B", "C");
        var module = new ModuleDef { Types = { alphabet } };

        var actualExpression = Parse("Alphabet.C", module);
        var actualExpressionByIndex = Parse("Alphabet[2]", module);
        var actualExpressionByName = Parse("Alphabet['C']", module);

        actualExpression.Should().BeOfType<MemberAccess>();
        actualExpression.As<MemberAccess>().Type.Should().Be(Type.Number);

        actualExpressionByIndex.Should().BeOfType<MemberIndex>();
        actualExpressionByIndex.As<MemberIndex>().Type.Should().Be(Type.String);

        actualExpressionByName.Should().BeOfType<MemberIndex>();
        actualExpressionByName.As<MemberIndex>().Type.Should().Be(Type.Number);
    }

    [Fact]
    void Member_access_operator_should_parse_properly()
    {
        var Book = Type.CreateAlias("Book", new MemberDefinition[]
        {
            new PropertyDefinition { Name = "id", Type = Type.Number }
        });
        var module = new ModuleDef
        {
            Types = { Book },
            Constants = { new ConstDeclaration { Name = "book", Type = Book } }
        };

        var actualExpression = Parse("book.id", module);

        actualExpression.Should().BeOfType<MemberAccess>();
        actualExpression.As<MemberAccess>().Instance.Should().BeOfType<Identifier>();
        actualExpression.As<MemberAccess>().Member.Should().BeOfType<PropertyDefinition>();
    }

    [Fact]
    void Nested_member_access_operator_should_parse_properly()
    {
        var Book = Type.CreateAlias("Book", new MemberDefinition[]
        {
            new PropertyDefinition { Name = "name", Type = Type.String }
        });
        var module = new ModuleDef
        {
            Types = { Book },
            Constants = { new ConstDeclaration { Name = "book", Type = Book } }
        };

        var actualExpression = Parse("book.name.length", module);

        actualExpression.Should().BeOfType<MemberAccess>();
        actualExpression.As<MemberAccess>().Instance.Should().BeOfType<MemberAccess>();
        actualExpression.As<MemberAccess>().Member.Should().BeOfType<PropertyDefinition>();
    }

    [Fact]
    void Optional_member_access_operator_should_parse_properly()
    {
        var Book = Type.CreateAlias("Book", new MemberDefinition[]
        {
            new PropertyDefinition { Name = "id", Type = Type.Number }
        });
        var module = new ModuleDef
        {
            Types = { Book },
            Constants = { new ConstDeclaration { Name = "book", Type = Book } }
        };

        var actualExpression = Parse("book?.id", module);

        actualExpression.Should().BeOfType<MemberAccess>();
        actualExpression.As<MemberAccess>().IsOptional.Should().BeTrue();
        actualExpression.As<MemberAccess>().Instance.Should().BeOfType<Identifier>();
        actualExpression.As<MemberAccess>().Member.Should().BeOfType<PropertyDefinition>();
    }

    [Fact]
    void Nested_optional_member_access_operator_should_parse_properly()
    {
        var Book = Type.CreateAlias("Book", new MemberDefinition[]
        {
            new PropertyDefinition { Name = "name", Type = Type.String }
        });
        var module = new ModuleDef
        {
            Types = { Book },
            Constants = { new ConstDeclaration { Name = "book", Type = Book } }
        };

        var actualExpression = Parse("book.name?.length", module);

        actualExpression.Should().BeOfType<MemberAccess>();
        actualExpression.As<MemberAccess>().IsOptional.Should().BeTrue();
        actualExpression.As<MemberAccess>().Instance.Should().BeOfType<MemberAccess>();
        actualExpression.As<MemberAccess>().Instance.As<MemberAccess>().Instance.Should().BeOfType<Identifier>();
        actualExpression.As<MemberAccess>().Member.Should().BeOfType<PropertyDefinition>();
    }

    [Fact]
    void Function_call_operator_should_parse_properly()
    {
        var module = new ModuleDef
        {
            Functions =
            {
                new FunctionDeclaration
                {
                    Name = "sum",
                    Parameters = { new FunctionParameter("args", Type.ArrayOf(Type.Any), isRest: true) }
                }
            }
        };

        var actualExpression = Parse("sum(1,2,3)", module);

        actualExpression.Should().BeOfType<FunctionCall>();
        actualExpression.As<FunctionCall>().Expression.Should().BeOfType<Identifier>()
            .Which.Subject.Should().BeOfType<FunctionDeclaration>();
        actualExpression.As<FunctionCall>().Arguments.Should().HaveCount(3);
    }

    [Fact]
    void Indexer_member_access_operator_should_parse_properly()
    {
        var module = new ModuleDef
        {
            Constants =
            {
                new ConstDeclaration { Name = "obj", Type = Type.Any },
                new ConstDeclaration { Name = "index", Type = Type.Number }
            }
        };

        var actualExpression = Parse("obj[index]()", module);

        actualExpression.Should().BeOfType<FunctionCall>();
        actualExpression.As<FunctionCall>().Expression.Should().BeOfType<MemberIndex>();
    }

    [Fact]
    void Optional_function_call_operator_should_parse_properly()
    {
        var module = new ModuleDef
        {
            Constants =
            {
                new ConstDeclaration { Name = "obj", Type = Type.Any },
                new ConstDeclaration { Name = "index", Type = Type.Number }
            }
        };

        var actualExpression = Parse("obj[index]?.()", module);

        actualExpression.Should().BeOfType<FunctionCall>();
        actualExpression.As<FunctionCall>().IsOptional.Should().BeTrue();
        actualExpression.As<FunctionCall>().Expression.Should().BeOfType<MemberIndex>();
    }

    [Fact]
    void Optional_indexer_member_access_operator_should_work_properly()
    {
        var module = new ModuleDef
        {
            Constants =
            {
                new ConstDeclaration { Name = "obj", Type = Type.Any },
                new ConstDeclaration { Name = "index", Type = Type.Number }
            }
        };

        var actualExpression = Parse("obj?.[index]()", module);

        actualExpression.Should().BeOfType<FunctionCall>();
        actualExpression.As<FunctionCall>().Expression.Should().BeOfType<MemberIndex>().Which
            .IsOptional.Should().BeTrue();
    }

    [Fact]
    void New_keyword_should_parse_properly()
    {
        var actualExpression = Parse("new Date(360000)");

        actualExpression.Should().BeOfType<New>();
    }

    [Fact]
    void Builtin_type_constructor_should_be_call_without_new_keyword()
    {
        var actualExpression = Parse("Function()");

        actualExpression.Should().BeOfType<FunctionCall>();
        actualExpression.As<FunctionCall>()
            .Expression.As<Identifier>()
            .Subject.As<FunctionDefinition>().IsConstructor.Should().BeTrue();
    }

    [Fact]
    void Non_builtin_type_constructor_call_without_new_keyword_should_throw_exception()
    {
        var module = new ModuleDef
        {
            Types = { Type.CreateAlias("Book", Enumerable.Empty<MemberDefinition>()) }
        };

        var parse = () => Parse("Book()", module);

        parse.Should().ThrowExactly<InvalidOperationException>();
    }

    [Fact]
    void Postfix_increment_operator_should_parse_properly()
    {
        var module = new ModuleDef
        {
            Vars = { new VarDeclaration { Name = "x", Type = Type.Number } }
        };

        var actualExpression = Parse("x++", module);

        actualExpression.Should().BeOfType<Increment>()
            .Which.IsPrefix.Should().BeFalse();
    }

    [Fact]
    void Postfix_decrement_operator_should_parse_properly()
    {
        var module = new ModuleDef
        {
            Vars = { new VarDeclaration { Name = "x", Type = Type.Number } }
        };

        var actualExpression = Parse("x--", module);

        actualExpression.Should().BeOfType<Decrement>()
            .Which.IsPrefix.Should().BeFalse();
    }

    [Fact]
    void Prefix_increment_operator_should_parse_properly()
    {
        var module = new ModuleDef
        {
            Vars = { new VarDeclaration { Name = "x", Type = Type.Number } }
        };

        var actualExpression = Parse("++x", module);

        actualExpression.Should().BeOfType<Increment>()
            .Which.IsPrefix.Should().BeTrue();
    }

    [Fact]
    void Prefix_decrement_operator_should_parse_properly()
    {
        var module = new ModuleDef
        {
            Vars = { new VarDeclaration { Name = "x", Type = Type.Number } }
        };

        var actualExpression = Parse("--x", module);

        actualExpression.Should().BeOfType<Decrement>()
            .Which.IsPrefix.Should().BeTrue();
    }

    Expression Parse(string text, ModuleDef? definitions = null)
    {
        var options = ParserOptions.GetDefault();
        if (definitions is not null)
            options.Modules.Add(definitions);
        return new ExpressionParser(text, options).Parse();
    }

    class Evaluator : ExpressionVisitor
    {
        public long Evaluate(Expression expression)
        {
            return Visit(expression) is Literal { Value: long value }
                ? value
                : throw new NotSupportedException();
        }

        protected override Expression Visit(BinaryOperator node)
        {
            if (node is not (Add or Subtract or Multiply or Divide))
                return base.Visit(node);

            long ApplyOperator(long x, long y, BinaryOperator @operator)
            {
                return @operator switch
                {
                    Add => x + y,
                    Subtract => x - y,
                    Multiply => x * y,
                    Divide => x / y
                };
            }

            var left = Visit(node.Left);
            var right = Visit(node.Right);
            return left is Literal { Value: long leftValue } && right is Literal { Value: long rightValue }
                ? Expression.Literal(ApplyOperator(leftValue, rightValue, node), Type.Number)
                : throw new NotSupportedException();
        }

        protected override Expression Visit(UnaryOperator node)
        {
            if (node is not (UnaryPlus or Negate))
                return base.Visit(node);

            long ApplyOperator(long x, UnaryOperator @operator)
            {
                return @operator switch
                {
                    UnaryPlus => x,
                    Negate => -x
                };
            }

            var operand = Visit(node.Operand);
            return operand is Literal { Value: long value }
                ? Expression.Literal(ApplyOperator(value, node), Type.Number)
                : throw new NotSupportedException();
        }
    }
}
