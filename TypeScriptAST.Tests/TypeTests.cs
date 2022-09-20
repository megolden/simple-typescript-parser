using FluentAssertions;
using Type = TypeScriptAST.Declarations.Types.Type;
using Xunit;

namespace TypeScriptAST.Tests;

public class TypeTests
{
    [Fact]
    void IsAssignable_should_work_properly()
    {
        Type.String.IsAssignableFrom(Type.String).Should().BeTrue();
        Type.String.IsAssignableFrom(Type.Number).Should().BeFalse();
    }

    [Fact]
    void IsAssignable_should_work_properly_with_alias_types()
    {
        var age = Type.CreateAlias("Age", Type.Number);

        age.IsAssignableFrom(Type.Number).Should().BeTrue();
        Type.Number.IsAssignableFrom(age).Should().BeTrue();
    }

    [Fact]
    void IsAssignable_should_work_properly_with_class_types()
    {
        var drawable = Type.CreateInterface("Drawable");
        var shape = Type.CreateClass("Shape", interfaces: drawable);
        var square = Type.CreateClass("Square", superType: shape);
        var rectangle = Type.CreateClass("Rectangle", superType: square);

        shape.IsAssignableFrom(square).Should().BeTrue();
        square.IsAssignableFrom(shape).Should().BeFalse();
        shape.IsAssignableFrom(rectangle).Should().BeTrue();
        square.IsAssignableFrom(rectangle).Should().BeTrue();
        rectangle.IsAssignableFrom(shape).Should().BeFalse();
        rectangle.IsAssignableFrom(square).Should().BeFalse();
        drawable.IsAssignableFrom(square).Should().BeTrue();
        drawable.IsAssignableFrom(rectangle).Should().BeTrue();
    }

    [Fact]
    void IsAssignable_should_work_properly_with_enum_types()
    {
        var digit = Type.CreateEnum("Digit", ("One", 1), ("Two", 2), ("Three", 3));
        var alphabet = Type.CreateEnum("Alphabet", ("A", "a"), ("B", "b"), ("C", "c"));

        digit.IsAssignableFrom(Type.Number).Should().BeTrue();
        digit.IsAssignableFrom(Type.String).Should().BeFalse();
        Type.Number.IsAssignableFrom(digit).Should().BeTrue();
        Type.String.IsAssignableFrom(digit).Should().BeFalse();
        alphabet.IsAssignableFrom(Type.Number).Should().BeFalse();
        alphabet.IsAssignableFrom(Type.String).Should().BeTrue();
        Type.String.IsAssignableFrom(alphabet).Should().BeTrue();
        Type.Number.IsAssignableFrom(alphabet).Should().BeFalse();
    }

    [Fact]
    void IsAssignable_should_work_properly_with_interface_types()
    {
        var ser = Type.CreateInterface("Serializable");
        var enm = Type.CreateInterface("Enumerable", interfaces: ser);
        var str = Type.CreateInterface("String", interfaces: enm);

        Type.Unknown.IsAssignableFrom(Type.Any).Should().BeTrue();
        Type.Any.IsAssignableFrom(Type.Boolean).Should().BeTrue();
        Type.String.IsAssignableFrom(Type.Number).Should().BeFalse();

        ser.IsAssignableFrom(enm).Should().BeTrue();
        enm.IsAssignableFrom(ser).Should().BeFalse();
        enm.IsAssignableFrom(str).Should().BeTrue();
        ser.IsAssignableFrom(str).Should().BeTrue();
    }

    [Fact]
    void IsAssignable_should_work_properly_with_function_types()
    {
        var isString = Type.FunctionOf(Type.Boolean, ("value", Type.Any, false, false));
        var isName = Type.FunctionOf(Type.Boolean, ("value", Type.String, false, false));
        var isNumber = Type.FunctionOf(Type.Boolean, ("value", Type.Any, false, false));
        var isNames = Type.FunctionOf(Type.Boolean, ("values", Type.ArrayOf(Type.String), false, true));
        var toString = Type.FunctionOf(Type.String, ("value", Type.Any, false, false));

        isString.IsAssignableFrom(isName).Should().BeTrue();
        isName.IsAssignableFrom(isString).Should().BeFalse();
        isNumber.IsAssignableFrom(isString).Should().BeTrue();
        isNames.IsAssignableFrom(isName).Should().BeTrue();
        isNames.IsAssignableFrom(isString).Should().BeFalse();
        isString.IsAssignableFrom(isNames).Should().BeFalse();
        toString.IsAssignableFrom(isString).Should().BeFalse();
    }

    [Fact]
    void IsAssignable_should_work_properly_with_union_types()
    {
        var booleanNumber = Type.UnionOf(Type.Boolean, Type.Number);
        var booleanNumberString = Type.UnionOf(Type.Boolean, Type.Number, Type.String);

        booleanNumber.IsAssignableFrom(Type.Boolean).Should().BeTrue();
        booleanNumber.IsAssignableFrom(Type.Number).Should().BeTrue();
        booleanNumber.IsAssignableFrom(Type.String).Should().BeFalse();
        booleanNumberString.IsAssignableFrom(booleanNumber).Should().BeTrue();
        booleanNumber.IsAssignableFrom(booleanNumberString).Should().BeFalse();
    }

    [Fact]
    void IsAssignable_should_work_properly_with_array_type()
    {
        var numberArray = Type.ArrayOf(Type.Number);
        var strArray = Type.ArrayOf(Type.String);
        var anyArray = Type.ArrayOf(Type.Any);

        numberArray.IsAssignableFrom(Type.ArrayOf(Type.Number)).Should().BeTrue();
        numberArray.IsAssignableFrom(anyArray).Should().BeFalse();
        anyArray.IsAssignableFrom(numberArray).Should().BeTrue();
        anyArray.IsAssignableFrom(strArray).Should().BeTrue();
        strArray.IsAssignableFrom(numberArray).Should().BeFalse();
    }
}
