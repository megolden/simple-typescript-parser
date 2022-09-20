using System;
using System.Collections.Generic;
using System.Linq;

namespace TypeScriptAST.Declarations.Types;

public abstract class Type : IEquatable<Type>
{
    public string Name { get; }
    public string? Namespace { get; }
    public Type? OwnerType { get; }

    public string FullName
    {
        get
        {
            return OwnerType is not null
                ? $"{OwnerType.FullName}.{Name}"
                : $"{(Namespace is { Length: > 0 } ns ? ns + "." : "")}{Name}";
        }
    }

    protected Type(string fullName, Type? ownerType = null)
    {
        OwnerType = ownerType;
        if (ownerType is not null)
        {
            Namespace = ownerType.Namespace;
            Name = fullName.LastIndexOf('.') is var sepIndex && sepIndex >= 0 ?
                   fullName.Substring(sepIndex + 1) :
                   fullName;
        }
        else
        {
            var sepIndex = fullName.LastIndexOf('.');
            Name = sepIndex >= 0 ? fullName.Substring(sepIndex + 1) : fullName;
            Namespace = sepIndex >= 0 ? fullName.Remove(sepIndex) : null;
        }
    }

    public override string ToString()
    {
        return Name;
    }

    protected virtual bool EqualTo(Type that)
    {
        return this.FullName.Equals(that.FullName, StringComparison.Ordinal);
    }

    bool IEquatable<Type>.Equals(Type? other) => other is not null && this.EqualTo(other);

    public sealed override bool Equals(object? other)
    {
        if (other is not Type that)
            return false;

        if (ReferenceEquals(this, that))
            return true;

        return this.EqualTo(that);
    }

    public override int GetHashCode()
    {
        return FullName.GetHashCode();
    }

    public virtual bool IsAssignableFrom(Type type)
    {
        if (this.Equals(type))
            return true;

        switch (type)
        {
            case AliasType aliasType when IsAssignableFrom(aliasType.ReferencedType):
            case EnumType enumType when IsAssignableFrom(enumType.ValueType):
                return true;
            default:
                return false;
        }
    }

    public static bool operator ==(Type x, Type y)
    {
        return System.Object.Equals(x, y);
    }

    public static bool operator !=(Type x, Type y)
    {
        return !(x == y);
    }

    public static Type operator |(Type x, Type y)
    {
        return UnionOf(x, y);
    }

    public static readonly Type Unknown = CreateInterface("unknown");
    public static readonly Type Never = CreateInterface("never");
    public static readonly Type Any = CreateInterface("any", Unknown);
    public static readonly Type Void = CreateInterface("void", Any);
    public static readonly Type Null = CreateInterface("null", Any);
    public static readonly Type Undefined = CreateInterface("undefined", Void);
    public static readonly Type StringType = CreateInterface("String", Any);
    public static readonly Type String = CreateAlias("string", StringType);
    public static readonly Type NumberType = CreateInterface("Number", Any);
    public static readonly Type Number = CreateAlias("number", NumberType);
    public static readonly Type BigIntType = CreateInterface("BigInt", Any);
    public static readonly Type BigInt = CreateAlias("bigint", BigIntType);
    public static readonly Type BooleanType = CreateInterface("Boolean", Any);
    public static readonly Type Boolean = CreateAlias("boolean", BooleanType);
    public static readonly Type ObjectType = CreateInterface("Object", Any);
    public static readonly Type Object = CreateAlias("object", ObjectType);
    public static readonly Type SymbolType = CreateInterface("Symbol", Any);
    public static readonly Type Symbol = CreateInterface("symbol", SymbolType);
    public static readonly Type RegExp = CreateInterface("RegExp", ObjectType);

    public static Type UnionOf(params Type[] types) => UnionType.UnionOrTypeOf(types);
    public static FunctionType FunctionOf(
        Type type,
        params (string Name, Type Type, bool IsOptional, bool IsRest)[] parameters)
    {
        return new FunctionType(
            parameters.Select(_ => new FunctionParameter(_.Name, _.Type, _.IsOptional, _.IsRest)),
            type);
    }
    public static ArrayType ArrayOf(Type elementType) => new ArrayType(elementType);

    public static EnumType CreateEnum(string fullName, params (string Name, object Value)[] values)
    {
        return new EnumType(fullName, values.ToDictionary(_ => _.Name, _ => _.Value));
    }
    public static EnumType CreateEnum(string fullName, params string[] names)
    {
        return CreateEnum(fullName, names.Select((name, index) => (name, (object)index)).ToArray());
    }

    public static AliasType CreateAlias(string fullName, Type referencedType)
    {
        return new AliasType(fullName, referencedType);
    }

    public static ClassType CreateClass(
        string fullName,
        IEnumerable<MemberDefinition> members,
        IEnumerable<Type> interfaces,
        bool isAbstract = false,
        Type? superType = null)
    {
        return new ClassType(fullName, isAbstract, members, interfaces.Cast<InterfaceType>(), superType);
    }
    public static ClassType CreateClass(
        string fullName,
        IEnumerable<MemberDefinition> members,
        bool isAbstract = false,
        Type? superType = null)
    {
        return CreateClass(fullName, members, Enumerable.Empty<InterfaceType>(), isAbstract, superType);
    }
    public static ClassType CreateClass(
        string fullName,
        bool isAbstract = false,
        Type? superType = null,
        params Type[] interfaces)
    {
        return CreateClass(fullName, Enumerable.Empty<MemberDefinition>(), interfaces, isAbstract, superType);
    }

    public static InterfaceType CreateInterface(
        string fullName,
        IEnumerable<MemberDefinition> members,
        IEnumerable<Type> interfaces)
    {
        return new InterfaceType(fullName, members, interfaces.Cast<InterfaceType>());
    }
    public static InterfaceType CreateInterface(string fullName, IEnumerable<MemberDefinition> members)
    {
        return CreateInterface(fullName, members, Enumerable.Empty<InterfaceType>());
    }
    public static InterfaceType CreateInterface(string fullName, params Type[] interfaces)
    {
        return CreateInterface(fullName, Enumerable.Empty<MemberDefinition>(), interfaces);
    }

    internal static string GenerateAnonymousName() => "anonymous_" + Guid.NewGuid().ToString("N").ToLower();
}
