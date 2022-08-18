namespace TypeScriptAST.Declarations.Types;

internal class Number : Type
{
    public Number() : base("number", Any) { }

    public override MemberDefinition[] DeclaredMembers => new MemberDefinition[]
    {
        new FunctionDefinition
        {
            Name = "toString",
            Type = String,
            Parameters =
            {
                new FunctionParameter("radix", Number, isOptional: true)
            },
            DeclaringType = this
        }
    };
}
