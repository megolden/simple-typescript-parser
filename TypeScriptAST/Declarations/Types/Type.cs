using System.Collections.Generic;
using System.Linq;

namespace TypeScriptAST.Declarations.Types;

public abstract class Type
{
    public string Name { get; private init; }
    public string? Namespace { get; private init; }
    public virtual bool IsInterface { get; } = false;
    public virtual bool IsClass { get; } = false;
    public virtual bool IsAbstract { get; } = false;
    public virtual bool IsEnum { get; } = false;
    public virtual bool IsArray { get; } = false;
    public virtual Type? UnderlyingType { get; } = null;
    public Type? SuperType { get; private init; }
    public virtual bool IsUnion { get; } = false;
    public virtual Type[] UnionTypes { get; } = System.Array.Empty<Type>();
    public virtual Type[] Interfaces { get; } = System.Array.Empty<Type>();
    public virtual MemberDefinition[] DeclaredMembers { get; } = System.Array.Empty<MemberDefinition>();
    public FunctionDefinition[] Constructors => Functions.Where(_ => _.IsConstructor).ToArray();
    public FunctionDefinition[] Functions => Members.OfType<FunctionDefinition>().ToArray();
    public PropertyDefinition[] Properties => Members.OfType<PropertyDefinition>().ToArray();
    public MemberDefinition[] Members => DeclaredMembers
        .Concat(SuperType?.Members ?? Enumerable.Empty<MemberDefinition>())
        .Concat(Interfaces.SelectMany(_ => _.Members))
        .ToArray();

    public string FullName => $"{(Namespace is { Length: > 0 } ns ? ns + "." : "")}{Name}";

    protected Type(string fullName, Type? superType = null)
    {
        (Name, Namespace) =
            fullName.LastIndexOf('.') is var sepIndex && sepIndex >= 0
            ? (fullName.Substring(sepIndex + 1), fullName.Remove(sepIndex))
            : (fullName, null);
        SuperType = superType;
    }

    public override string ToString()
    {
        return Name;
    }

    public override bool Equals(object? other)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return other is Type that &&
               this.FullName.Length > 0 && that.FullName.Length > 0 &&
               this.FullName == that.FullName;
    }

    public override int GetHashCode()
    {
        return FullName.Length > 0 ? FullName.GetHashCode() : base.GetHashCode();
    }

    public virtual bool IsAssignableFrom(Type type)
    {
        if (ReferenceEquals(type, this) || type == this)
            return true;
        if (type.IsEnum && IsAssignableFrom(type.UnderlyingType))
            return true;
        if (IsEnum && type.UnderlyingType.IsAssignableFrom(type))
            return true;
        if (IsUnion && UnionTypes.Any(uType => uType.IsAssignableFrom(type)))
            return true;
        if (type.Interfaces.Any(IsAssignableFrom))
            return true;
        return type.SuperType is not null && IsAssignableFrom(type.SuperType);
    }

    public static bool operator ==(Type x, Type y)
    {
        return System.Object.Equals(x, y);
    }

    public static bool operator !=(Type x, Type y)
    {
        return !System.Object.Equals(x, y);
    }

    public static Type operator |(Type x, Type y)
    {
        return UnionOf(x, y);
    }

    public static readonly Type Unknown = new Unknown();
    public static readonly Type Never = new Never();
    public static readonly Type Any = new Any();
    public static readonly Type Void = new Void();
    public static readonly Type Null = new Null();
    public static readonly Type Undefined = new Undefined();
    public static readonly Type String = new String();
    public static readonly Type StringConstructor = new StringConstructor();
    public static readonly Type Number = new Number();
    public static readonly Type NumberConstructor = new NumberConstructor();
    public static readonly Type BigInt = new BigInt();
    public static readonly Type Boolean = new Boolean();
    public static readonly Type BooleanConstructor = new BooleanConstructor();
    public static readonly Type Object = new Object();
    public static readonly Type ObjectConstructor = new ObjectConstructor();
    public static readonly Type FunctionConstructor = new FunctionConstructor();
    public static readonly Type RegExp = new RegExp();
    public static readonly Type Date = new Date();
    public static readonly Type Console = new Console();
    public static readonly Type Symbol = new Symbol();
    public static readonly Type SymbolConstructor = new SymbolConstructor();
    public static readonly Type Error = new Error();
    public static readonly Type Math = new Math();
    public static readonly Type Json = new Json();
    public static readonly Type ArrayConstructor = new ArrayConstructor();
    public static Type ArrayOf(Type elementType) => new Array(elementType);
    public static Type UnionOf(params Type[] types) => new UnionType(types);
    public static Type FunctionOf(
        Type type,
        params (string Name, Type Type, bool IsOptional, bool IsRest)[] parameters)
    {
        return new Function(
            parameters.Select(_ => new Function.Parameter(_.Name, _.Type, _.IsOptional, _.IsRest)),
            type);
    }

    public static Type CreateEnum(string fullName, params (string Name, object Value)[] values)
    {
        return new EnumType(fullName, values.ToDictionary(_ => _.Name, _ => _.Value));
    }
    public static Type CreateEnum(string fullName, params string[] names)
    {
        return CreateEnum(fullName, names.Select((name, index) => (name, (object)index)).ToArray());
    }

    public static Type CreateAlias(string fullName, IEnumerable<MemberDefinition> members)
    {
        return new AliasType(fullName, members);
    }

    public static Type CreateClass(
        string fullName,
        bool isAbstract,
        IEnumerable<MemberDefinition> members,
        IEnumerable<Type> interfaces,
        Type? superType = null)
    {
        return new ClassType(fullName, isAbstract, members, interfaces, superType);
    }
    public static Type CreateClass(
        string fullName,
        bool isAbstract,
        IEnumerable<MemberDefinition> members,
        Type? superType = null)
    {
        return CreateClass(fullName, isAbstract, members, Enumerable.Empty<Type>(), superType);
    }

    public static Type CreateInterface(
        string fullName,
        IEnumerable<MemberDefinition> members,
        IEnumerable<Type> interfaces)
    {
        return new InterfaceType(fullName, members, interfaces);
    }
    public static Type CreateInterface(string fullName, IEnumerable<MemberDefinition> members)
    {
        return CreateInterface(fullName, members, Enumerable.Empty<Type>());
    }
}
