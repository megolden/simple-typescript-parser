using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using TypeScriptAST.Declarations;
using Xunit;

namespace TypeScriptAST.Tests;

public class ParserStatementTests
{
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
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData("\r")]
    [InlineData("\n")]
    void Empty_or_white_space_only_expressions_should_skipped(string expression)
    {
        var actualExpression = Parse(expression);

        actualExpression.Should().BeOfType<StatementList>()
            .Which.IsEmpty.Should().BeTrue();
    }

    [Theory]
    [InlineData("// single line comment")]
    [InlineData("/* block comment */")]
    [InlineData("/* multiline block comment \n it is second line */")]
    void Comment_only_expressions_parse_should_skipped(string expression)
    {
        var actualExpression = Parse(expression);

        actualExpression.Should().BeOfType<StatementList>()
            .Which.IsEmpty.Should().BeTrue();
    }

    [Fact]
    void Xml_comment_should_parse_properly()
    {
        var actualExpressions = Parse(@"
/// <reference lib=""es5"" no-default-lib=""true"" />
/// <reference path=""test.ts"" />
/// <amd-module />
");

        actualExpressions.Should().BeOfType<StatementList>().Which.Count.Should().Be(3);
        actualExpressions.As<StatementList>().Should().AllBeOfType<XmlComment>();

        actualExpressions.As<StatementList>().First().As<XmlComment>().Name.Should().Be("reference");
        actualExpressions.As<StatementList>().First().As<XmlComment>().Properties.Should()
            .BeEquivalentTo(new Dictionary<string, string>
            {
                { "lib", "es5" },
                { "no-default-lib", "true" }
            });

        actualExpressions.As<StatementList>().ElementAt(1).As<XmlComment>().Name.Should().Be("reference");
        actualExpressions.As<StatementList>().ElementAt(1).As<XmlComment>().Properties.Should()
            .BeEquivalentTo(new Dictionary<string, string>
            {
                { "path", "test.ts" }
            });

        actualExpressions.As<StatementList>().Last().As<XmlComment>().Name.Should().Be("amd-module");
        actualExpressions.As<StatementList>().Last().As<XmlComment>().Properties.Should().BeEmpty();
    }

    [Fact]
    void Empty_statement_should_parse_properly()
    {
        var actualExpression = Parse(";");

        actualExpression.Should().BeOfType<EmptyStatement>();
    }

    Statement Parse(string text, ModuleDef? definitions = null)
    {
        var options = ParserOptions.GetDefault();
        if (definitions is not null)
            options.TypeSystem.AddModules(definitions);
        return new Parser(text, options).Parse();
    }
}
