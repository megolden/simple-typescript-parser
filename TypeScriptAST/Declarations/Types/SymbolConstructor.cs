namespace TypeScriptAST.Declarations.Types;

internal class SymbolConstructor : Constructor
{
    public SymbolConstructor() : base("Symbol")
    {
    }

    public override MemberDefinition[] DeclaredMembers => new MemberDefinition[]
    {
        new FunctionDefinition
        {
            Name = "for",
            Type = Symbol,
            IsStatic = true,
            Parameters =
            {
                new FunctionParameter("key", String)
            },
            DeclaringType = this
        }
    };
}
