using System.Linq;
using FluentAssertions;
using Sdcb.TypeScript.TsTypes;
using Xunit;

namespace TypeScriptAST.Tests;

public class SdcbTests
{
    [Fact]
    void Simple_function_declaration_should_be_parse_properly()
    {
        var ast = new Sdcb.TypeScript.TypeScriptAST(@"function f(n: number/*XYZ*/): boolean {}");

        var stmts = (ast.RootNode as SourceFile).Statements;

        stmts.Should().HaveCount(1);
        stmts.First().Should().BeOfType<FunctionDeclaration>();
        stmts.First().As<FunctionDeclaration>().Type.Kind.Should().Be(SyntaxKind.BooleanKeyword);
        stmts.First().As<FunctionDeclaration>().Parameters.First().Type.Kind.Should().Be(SyntaxKind.NumberKeyword);
    }
}
