using System;
using FluentAssertions;
using TypeScriptAST.Declarations;
using Xunit;

namespace TypeScriptAST.Tests;

public class ParserDeclarationTests
{
    [Fact]
    void Test()
    {
        var actualExpression = Parse("declare namespace test { }");

        actualExpression.Should().NotBeNull();
    }

    Declaration Parse(string text)
    {
        var options = ParserOptions.GetDefault();
        return new Parser(text, options).Parse()
            as Declaration ?? throw new Exception("the statement is not a Declaration");
    }
}
