namespace TypeScriptAST.Declarations.Types;

internal class StringConstructor : Constructor
{
    public StringConstructor() : base("String")
    {
    }

    public override MemberDefinition[] DeclaredMembers => new MemberDefinition[]
    {
        new FunctionDefinition
        {
            Name = FunctionDefinition.ConstructorName,
            Type = String,
            Parameters =
            {
                new FunctionParameter("value", Any, isOptional: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "fromCharCode",
            Type = String,
            Parameters =
            {
                new FunctionParameter("codes", ArrayOf(Number), isRest: true)
            },
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "fromCodePoint",
            Type = String,
            Parameters =
            {
                new FunctionParameter("codePoints", ArrayOf(Number), isRest: true)
            },
            IsStatic = true,
            DeclaringType = this
        }
    };
}
