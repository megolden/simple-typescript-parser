using System.Collections.Generic;
using System.Linq;

namespace TypeScriptAST.Declarations.Types;

public class ClassType : Type
{
    private readonly List<MemberDefinition> _declaredMembers;
    private readonly List<InterfaceType> _interfaces;

    public bool IsAbstract { get; }
    public Type? SuperType { get; }

    public IReadOnlyCollection<MemberDefinition> DeclaredMembers => _declaredMembers.ToArray();
    public IReadOnlyCollection<InterfaceType> Interfaces => _interfaces.ToArray();

    internal ClassType(
        string fullName,
        bool isAbstract,
        IEnumerable<MemberDefinition> members,
        IEnumerable<InterfaceType> interfaces,
        Type? superType = null,
        Type? ownerType = null) : base(fullName, ownerType)
    {
        IsAbstract = isAbstract;
        SuperType = superType;
        _interfaces = interfaces.ToList();
        _declaredMembers = members
            .Select(_ => _.WithDeclaringType(this))
            .Select(_ => _ is FunctionMember { IsClassConstructor: true } ? _.WithType(this) : _)
            .ToList();
    }

    internal ClassType(string fullName, bool isAbstract, Type? superType = null) : this(
        fullName,
        isAbstract,
        Enumerable.Empty<MemberDefinition>(),
        Enumerable.Empty<InterfaceType>(),
        superType)
    {
    }

    public override bool IsAssignableFrom(Type type)
    {
        return base.IsAssignableFrom(type) ||
               (type is ClassType { SuperType: { } superType } && IsAssignableFrom(superType));
    }
}
