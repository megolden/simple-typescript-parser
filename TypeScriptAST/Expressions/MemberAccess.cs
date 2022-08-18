using TypeScriptAST.Declarations;

namespace TypeScriptAST.Expressions;

public class MemberAccess : Operator
{
    public Expression Instance { get; private init; }
    public IMemberInfo Member { get; private init; }
    public bool IsOptional { get; private init; }

    internal MemberAccess(Expression instance, IMemberInfo member, bool isOptional)
        : base(
            isOptional ? OperatorType.OptionalMemberAccess : OperatorType.MemberAccess,
            isOptional ? "?." : ".",
            member.Type)
    {
        Instance = instance;
        Member = member;
        IsOptional = isOptional;
    }

    public override string ToString()
    {
        return $"{Instance}{(IsOptional ? "?." : ".")}{Member.Name}";
    }
}
