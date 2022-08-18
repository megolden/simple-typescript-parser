using System.Collections.Generic;
using System.Linq;

namespace TypeScriptAST.Declarations.Types;

internal class ClassType : Type
{
    public sealed override bool IsClass => true;
    public sealed override bool IsAbstract { get; }
    public override MemberDefinition[] DeclaredMembers { get; }
    public override Type[] Interfaces { get; }

    public ClassType(
        string fullName,
        bool isAbstract,
        IEnumerable<MemberDefinition> members,
        IEnumerable<Type> interfaces,
        Type? superType = null) : base(fullName, superType ?? Object)
    {
        IsAbstract = isAbstract;
        Interfaces = interfaces.ToArray();
        DeclaredMembers = members.Select(member =>
        {
            member.DeclaringType = this;
            return member;
        }).ToArray();
    }

    public ClassType(string fullName, bool isAbstract, Type? superType = null) : this(
        fullName,
        isAbstract,
        Enumerable.Empty<MemberDefinition>(),
        Enumerable.Empty<Type>(),
        superType) { }
}
