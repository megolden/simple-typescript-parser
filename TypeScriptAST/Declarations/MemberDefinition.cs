using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Declarations;

public abstract class MemberDefinition : IMemberInfo
{
    public Type? DeclaringType { get; set; }
    public string Name { get; set; }
    public Type Type { get; set; }
    public bool IsStatic { get; set; }
    public string FullName => $"{(DeclaringType is not null ? DeclaringType.FullName + '.' : "")}{Name}";
}
