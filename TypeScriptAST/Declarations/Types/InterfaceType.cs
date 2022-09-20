using System.Collections.Generic;
using System.Linq;

namespace TypeScriptAST.Declarations.Types;

public class InterfaceType : Type
{
    private readonly List<MemberDefinition> _declaredMembers;
    private readonly List<InterfaceType> _interfaces;

    public IReadOnlyCollection<MemberDefinition> DeclaredMembers => _declaredMembers.ToArray();
    public IReadOnlyCollection<InterfaceType> Interfaces => _interfaces.ToArray();

    internal InterfaceType(
        string fullName,
        IEnumerable<MemberDefinition> members,
        IEnumerable<InterfaceType> interfaces,
        Type? ownerType = null) : base(fullName, ownerType)
    {
        _interfaces = interfaces.ToList();
        _declaredMembers = members.Select(_ => _.WithDeclaringType(this)).ToList();
    }

    internal InterfaceType(string fullName, Type? ownerType = null)
        : this(fullName, Enumerable.Empty<MemberDefinition>(), Enumerable.Empty<InterfaceType>(), ownerType)
    {
    }

    public override bool IsAssignableFrom(Type type)
    {
        if (base.IsAssignableFrom(type))
            return true;

        switch (type)
        {
            case InterfaceType interfaceType when interfaceType._interfaces.Any(IsAssignableFrom):
            case ClassType classType when classType.Interfaces.Any(IsAssignableFrom):
                return true;
            case ClassType { SuperType: { } } classType when IsAssignableFrom(classType.SuperType):
                return true;
            default:
                return false;
        }
    }
}
