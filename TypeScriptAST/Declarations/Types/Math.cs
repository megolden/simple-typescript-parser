namespace TypeScriptAST.Declarations.Types;

internal class Math : Type
{
    public Math() : base("Math", Object)
    {
    }

    public override MemberDefinition[] DeclaredMembers => new MemberDefinition[]
    {
        new PropertyDefinition
        {
            Name = "E",
            Type = Number,
            InitialValue = System.Math.E,
            IsStatic = true,
            ReadOnly = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "floor",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("x", Number)
            },
            IsStatic = true,
            DeclaringType = this
        }
    };
}
