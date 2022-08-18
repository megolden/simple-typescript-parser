namespace TypeScriptAST.Declarations.Types;

internal class Boolean : Type
{
    public Boolean() : base("boolean", Any)
    {
    }

    public override MemberDefinition[] DeclaredMembers => new MemberDefinition[]
    {
        new FunctionDefinition
        {
            Name = "valueOf",
            Type = this,
            DeclaringType = this
        }
    };
}
