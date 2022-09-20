using System;
using System.Collections.Generic;
using System.Linq;

namespace TypeScriptAST.Declarations.Types;

public class TypeSystem
{
    public List<ModuleDef> Modules { get; } = new();

    public void AddModules(params ModuleDef[] modules)
    {
        Modules.AddRange(modules);
    }

    private string PrepareNamespace(string fullName)
    {
        return
            !String.IsNullOrEmpty(GlobalModule.Namespace) &&
            fullName.StartsWith(GlobalModule.Namespace + '.') ?
            fullName.Substring(GlobalModule.Namespace.Length + 1) :
            fullName;
    }

    public bool NamespaceExists(string? name)
    {
        if (name is null)
            return true;

        if (!String.IsNullOrEmpty(GlobalModule.Namespace) &&
            String.Equals(name, GlobalModule.Namespace, StringComparison.Ordinal))
            return true;

        name = PrepareNamespace(name);
        var declarationsNamespaces = Modules
            .SelectMany(_ => _.Declarations.Select(_ => _.Namespace))
            .Distinct(StringComparer.Ordinal);
        var typesNamespaces = Modules
            .SelectMany(_ => _.Types.Select(_ => _.Namespace))
            .Distinct(StringComparer.Ordinal);
        return declarationsNamespaces.Concat(typesNamespaces)
            .Distinct(StringComparer.Ordinal)
            .Where(_ => _ is not null)
            .Select(PrepareNamespace)
            .Any(_ => String.Equals(_, name, StringComparison.Ordinal));
    }

    public Type? GetType(string fullName)
    {
        var allTypes = Modules.SelectMany(_ => _.Types);
        return allTypes.LastOrDefault(_ => PrepareNamespace(_.FullName) == PrepareNamespace(fullName));
    }

    public IEnumerable<IMemberInfo> GetMember(string fullName)
    {
        var declarations = GetDeclarations();
        var typesMembers = Modules.SelectMany(_ => _.Types.SelectMany(_ => GetMembers(_, declaredOnly: true)));
        return declarations.Concat(typesMembers)
            .Where(_ => PrepareNamespace(ResolveConstructorName(_)) == PrepareNamespace(fullName));

        string ResolveConstructorName(IMemberInfo member)
        {
            return member switch
            {
                FunctionMember { IsConstructor: true, DeclaringType: { } ctorDeclaringType } =>
                    ctorDeclaringType.FullName,
                FunctionMember { Name: "new" or "", DeclaringType: { } declaringType } =>
                    declaringType.FullName,
                _ => member.FullName
            };
        }
    }

    public IEnumerable<IMemberInfo> GetDeclaration(string fullName)
    {
        return GetDeclarations().Where(_ => PrepareNamespace(_.FullName) == PrepareNamespace(fullName));
    }

    private IEnumerable<IMemberInfo> GetDeclarations()
    {
        return Modules.SelectMany(_ => _.Declarations);
    }

    public static IEnumerable<MemberDefinition> GetMember(Type type, string name)
    {
        return GetMembers(type).Where(_ => _.Name.Equals(name, StringComparison.Ordinal));
    }

    public static IEnumerable<MemberDefinition> GetMembers(
        Type type,
        bool publicOnly = false,
        bool declaredOnly = false,
        bool functionOnly = false,
        bool propertyOnly = false)
    {
        bool FilterMember(MemberDefinition member, Type type)
        {
            return (!publicOnly || member.Modifier.HasFlag(MemberModifier.Public)) &&
                   (!declaredOnly || member.DeclaringType == type) &&
                   (!functionOnly || member is FunctionMember) &&
                   (!propertyOnly || member is PropertyMember);
        }

        switch (type)
        {
            case ClassType c: return
                c.DeclaredMembers
                .Concat(c.SuperType is not null ?
                    GetMembers(c.SuperType, publicOnly, declaredOnly, functionOnly, propertyOnly) :
                    Enumerable.Empty<MemberDefinition>())
                .Concat(c.Interfaces.SelectMany(_ =>
                    GetMembers(_, publicOnly, declaredOnly, functionOnly, propertyOnly)))
                .Where(member => FilterMember(member, type));

            case InterfaceType i: return
                i.DeclaredMembers.Concat(i.Interfaces.SelectMany(_ =>
                    GetMembers(_, publicOnly, declaredOnly, functionOnly, propertyOnly)))
                .Where(member => FilterMember(member, type));

            case EnumType e: return
                e.Members
                .Where(member => FilterMember(member, type));

            case AliasType a: return
                GetMembers(a.ReferencedType, publicOnly, declaredOnly, functionOnly, propertyOnly)
                .Where(member => FilterMember(member, type));

            case UnionType u:
                var unionTypesMembers = u.UnionTypes.ToDictionary(
                    uType => uType,
                    uType => GetMembers(uType, publicOnly, declaredOnly, functionOnly, propertyOnly)
                            .Where(_ => !(_ is FunctionMember f && (f.IsConstructor || f.HasNoName))));

                var firstTypeMembers = unionTypesMembers.First().Value;
                return firstTypeMembers
                    .Where(member => unionTypesMembers.Values.All(ms => ms.Any(_ =>
                        _.GetType() == member.GetType() &&
                        _.Name.Equals(member.Name, StringComparison.Ordinal))))
                    .Select(member =>
                    {
                        var members = unionTypesMembers.Values.SelectMany(m => m
                            .Where(_ =>
                                _.GetType() == member.GetType() &&
                                _.Name.Equals(member.Name, StringComparison.Ordinal)));
                        var types = members.Select(_ => _.Type).ToArray();
                        return member.Clone().WithType(Type.UnionOf(types)).WithDeclaringType(type);
                    });

            default: return
                Enumerable.Empty<MemberDefinition>();
        }
    }

    public static Type? GetElementType(Type type)
    {
        return type switch
        {
            ArrayType arrayType => arrayType.ElementType,
            _ => null
        };
    }

    public static bool IsArray(Type type)
    {
        return type switch
        {
            ArrayType => true,
            _ => false
        };
    }

    public static readonly Type[] BuiltInTypes =
    {
        Type.Void,
        Type.Null,
        Type.Undefined,
        Type.Unknown,
        Type.Never,
        Type.Any,
        Type.StringType,
        Type.String,
        Type.NumberType,
        Type.Number,
        Type.BigIntType,
        Type.BigInt,
        Type.BooleanType,
        Type.Boolean,
        Type.ObjectType,
        Type.Object,
        Type.SymbolType,
        Type.Symbol
    };
}
