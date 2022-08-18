namespace TypeScriptAST.Tests;

using Xunit;
using FluentAssertions;

public class UndefinedTests
{
    [Fact]
    void ToString_should_return_empty_string()
    {
        var result = Undefined.Value.ToString();

        result.Should().BeEmpty();
    }

    [Fact]
    void CompareTo_should_be_zero_when_Undefined_passed()
    {
        var typedResult = Undefined.Value.CompareTo(Undefined.Value);
        var objectResult = Undefined.Value.CompareTo((object)Undefined.Value);

        typedResult.Should().Be(0);
        objectResult.Should().Be(0);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(1)]
    [InlineData("")]
    void CompareTo_should_be_less_than_zero_when_Undefined_not_passed(object? value)
    {
        var result = Undefined.Value.CompareTo(value);

        result.Should().Be(-1);
    }

    [Fact]
    void Equals_should_return_true_when_Undefined_passed()
    {
        var typedResult = Undefined.Value.Equals(Undefined.Value);
        var objectResult = Undefined.Value.Equals((object)Undefined.Value);

        typedResult.Should().BeTrue();
        objectResult.Should().BeTrue();
    }

    [Fact]
    void Equals_should_return_false_when_Undefined_not_passed()
    {
        var nullResult = Undefined.Value.Equals(null);
        var valueResult = Undefined.Value.Equals(1);

        nullResult.Should().BeFalse();
        valueResult.Should().BeFalse();
    }
}
