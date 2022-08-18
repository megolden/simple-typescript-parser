namespace TypeScriptAST.Declarations.Types;

internal class NumberConstructor : Constructor
{
    public NumberConstructor() : base("Number") { }

    public override MemberDefinition[] DeclaredMembers => new MemberDefinition[]
    {
        new PropertyDefinition
        {
            Name = "NaN",
            Type = Number,
            IsStatic = true,
            ReadOnly = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "isInteger",
            Type = Boolean,
            Parameters =
            {
                new FunctionParameter("number", Unknown)
            },
            IsStatic = true,
            DeclaringType = this
        }
    };
}
