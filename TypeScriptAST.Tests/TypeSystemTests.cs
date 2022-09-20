using System;
using System.Linq;
using FluentAssertions;
using TypeScriptAST.Declarations;
using TypeScriptAST.Declarations.Types;
using Type = TypeScriptAST.Declarations.Types.Type;
using Xunit;

namespace TypeScriptAST.Tests;

public class TypeSystemTests
{
    [Fact]
    void Union_types_should_contain_intersect_members_of_underlying_types()
    {
        var a = Type.CreateInterface("A", new MemberDefinition[]
        {
            new PropertyMember("abc", Type.Any),
            new PropertyMember("a", Type.Any),
            new PropertyMember("ab", Type.String),
            new PropertyMember("code", Type.String),
            new FunctionMember("getName", Type.String)
        });
        var b = Type.CreateInterface("B", new MemberDefinition[]
        {
            new PropertyMember("abc", Type.String),
            new PropertyMember("b", Type.Any),
            new PropertyMember("ab", Type.Number),
            new PropertyMember("code", Type.String),
            new FunctionMember("getName", Type.String)
        });
        var c = Type.CreateInterface("C", new MemberDefinition[]
        {
            new PropertyMember("abc", Type.Number),
            new PropertyMember("c", Type.Any),
            new FunctionMember("code", Type.String),
            new FunctionMember("getName", Type.String, new[] { new FunctionParameter("def", Type.String) })
        });
        var u = Type.UnionOf(a, b, c);

        var members = TypeSystem.GetMembers(u).ToList();

        members.Should().HaveCount(2);
        members.Should().ContainSingle(_ => _ is PropertyMember);
        members.OfType<PropertyMember>().First().Should().BeEquivalentTo(new
        {
            Name = "abc",
            Type = Type.UnionOf(Type.Any, Type.String, Type.Number),
            DeclaringType = u
        });
        members.Should().ContainSingle(_ => _ is FunctionMember);
        members.OfType<FunctionMember>().First().Should().BeEquivalentTo(new
        {
            Name = "getName",
            Type = Type.String,
            Parameters = Array.Empty<FunctionParameter>(),
            DeclaringType = u
        });
    }
}
