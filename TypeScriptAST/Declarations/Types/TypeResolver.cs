using System.Collections.Generic;
using System.Linq;

namespace TypeScriptAST.Declarations.Types;

public class TypeResolver
{
    private readonly IReadOnlyList<ModuleDef> _modules;

    public TypeResolver(IEnumerable<ModuleDef> modules)
    {
        _modules = modules.ToList();
    }

    private string PrepareNamespace(string fullName)
    {
        return
            !System.String.IsNullOrEmpty(GlobalModule.Namespace) &&
            fullName.StartsWith(GlobalModule.Namespace + '.') ?
            fullName.Substring(GlobalModule.Namespace.Length + 1) :
            fullName;
    }

    private string? ExtractNamespace(string typeFullName)
    {
        var prepared = PrepareNamespace(typeFullName);
        var sepIndex = prepared.LastIndexOf('.');
        return sepIndex >= 0 ? prepared.Remove(sepIndex) : null;
    }

    public bool NamespaceExists(string? name)
    {
        if (name is null)
            return true;

        if (!System.String.IsNullOrEmpty(GlobalModule.Namespace) && name == GlobalModule.Namespace)
            return true;

        var membersNamespaces = _modules
            .SelectMany(_ => _.Members.Select(_ => _.FullName).Select(ExtractNamespace))
            .Distinct();
        var typesNamespaces = _modules
            .SelectMany(_ => _.Types.Select(_ => _.Namespace))
            .Distinct();
        return membersNamespaces.Concat(typesNamespaces)
            .Distinct()
            .Where(_ => _ is not null)
            .Any(ns => ns == name || ns.StartsWith(name + '.') || ns.EndsWith('.' + name));
    }

    public Type? ResolveType(string name)
    {
        var allTypes = _modules.SelectMany(_ => _.Types);
        return allTypes.LastOrDefault(_ => PrepareNamespace(_.FullName) == PrepareNamespace(name));
    }

    public IMemberInfo? ResolveMember(string fullName)
    {
        return ResolveMembers(fullName).FirstOrDefault();
    }

    public IEnumerable<IMemberInfo> ResolveMembers(string fullName)
    {
        var modulesMembers = _modules.SelectMany(_ => _.Members);
        var typesMembers = _modules.SelectMany(_ => _.Types.SelectMany(_ => _.Members));
        return modulesMembers.Concat(typesMembers)
              .Where(_ => PrepareNamespace(ResolveConstructorName(_)) == PrepareNamespace(fullName));

        string ResolveConstructorName(IMemberInfo member)
        {
            return member is FunctionDefinition { IsConstructor: true } ctor
                ? ctor.DeclaringType.FullName
                : member.FullName;
        }
    }

    public static MemberDefinition? GetMember(Type type, string name)
    {
        return type.Members.FirstOrDefault(_ => _.Name == name);
    }

    public static readonly Type[] BuiltInTypes =
    {
        Type.Void,
        Type.Null,
        Type.Undefined,
        Type.Unknown,
        Type.Never,
        Type.Any,
        Type.String,
        Type.StringConstructor,
        Type.Number,
        Type.NumberConstructor,
        Type.BigInt,
        Type.Boolean,
        Type.BooleanConstructor,
        Type.Object,
        Type.ObjectConstructor,
        Type.FunctionConstructor,
        Type.ArrayConstructor,
        Type.Symbol,
        Type.SymbolConstructor,
        Type.Error,
        Type.Math,
        Type.Json
    };
}
