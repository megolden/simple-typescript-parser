namespace TypeScriptAST.Declarations.Types;

internal class Error : Type
{
    public Error() : base("Error", Object)
    {
    }

    public override MemberDefinition[] DeclaredMembers => new MemberDefinition[]
    {
        new PropertyDefinition
        {
            Name = "name",
            Type = String,
            DeclaringType = this
        },
        new PropertyDefinition
        {
            Name = "message",
            Type = String,
            DeclaringType = this
        },
        new PropertyDefinition
        {
            Name = "stack",
            Type = String,
            IsOptional = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = FunctionDefinition.ConstructorName,
            Type = this,
            Parameters =
            {
                new FunctionParameter("message", String, isOptional: true)
            },
            DeclaringType = this
        }
    };
}
