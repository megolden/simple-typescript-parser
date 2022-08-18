using System.Collections.Generic;
using System.Linq;

namespace TypeScriptAST.Declarations.Types;

internal class AliasType : Type
{
    public override MemberDefinition[] DeclaredMembers { get; }

    public AliasType(string fullName, IEnumerable<MemberDefinition> members) : base(fullName, Object)
    {
        DeclaredMembers = members.Select(member =>
        {
            member.DeclaringType = this;
            return member;
        }).ToArray();
    }

    public AliasType(IEnumerable<MemberDefinition> members) : this(System.String.Empty, members)
    {
    }
}
