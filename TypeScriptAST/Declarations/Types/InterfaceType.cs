using System.Collections.Generic;
using System.Linq;

namespace TypeScriptAST.Declarations.Types;

internal class InterfaceType : Type
{
    public sealed override bool IsInterface => true;
    public override MemberDefinition[] DeclaredMembers { get; }
    public override Type[] Interfaces { get; }

    public InterfaceType(
        string fullName,
        IEnumerable<MemberDefinition> members,
        IEnumerable<Type> interfaces) : base(fullName, Object)
    {
        Interfaces = interfaces.ToArray();
        DeclaredMembers = members.Select(member =>
        {
            member.DeclaringType = this;
            return member;
        }).ToArray();
    }

    public InterfaceType(string fullName)
        : this(fullName, Enumerable.Empty<MemberDefinition>(), Enumerable.Empty<Type>()) { }
}
