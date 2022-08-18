namespace TypeScriptAST.Declarations.Types;

internal class RegExp : ClassType
{
    public RegExp() : base("RegExp", isAbstract: false) { }

    public override MemberDefinition[] DeclaredMembers => new MemberDefinition[]
    {
        new FunctionDefinition
        {
            Name = FunctionDefinition.ConstructorName,
            Type = this,
            Parameters =
            {
                new FunctionParameter("pattern", String | RegExp),
                new FunctionParameter("flags", String, isOptional: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "test",
            Type = Boolean,
            Parameters =
            {
                new FunctionParameter("string", String)
            },
            DeclaringType = this
        }
    };
}
