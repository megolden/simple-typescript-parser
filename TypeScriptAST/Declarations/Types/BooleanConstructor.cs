namespace TypeScriptAST.Declarations.Types;

internal class BooleanConstructor : Constructor
{
    public BooleanConstructor() : base("Boolean")
    {
    }

    public override MemberDefinition[] DeclaredMembers => new MemberDefinition[]
    {
        new FunctionDefinition
        {
            Name = FunctionDefinition.ConstructorName,
            Type = Boolean,
            Parameters =
            {
                new FunctionParameter("value", Unknown, isOptional: true)
            },
            DeclaringType = this
        }
    };
}
