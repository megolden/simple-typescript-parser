namespace TypeScriptAST.Declarations.Types;

internal class FunctionConstructor : Constructor
{
    public FunctionConstructor() : base("Function")
    {
    }

    public override MemberDefinition[] DeclaredMembers => new MemberDefinition[]
    {
        new FunctionDefinition
        {
            Name = FunctionDefinition.ConstructorName,
            Type = FunctionOf(Any, ("args", ArrayOf(Any), IsOptional: false, IsRest: true)),
            Parameters =
            {
                new FunctionParameter("args", ArrayOf(String), isRest: true)
            },
            DeclaringType = this
        }
    };
}
