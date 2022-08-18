namespace TypeScriptAST.Declarations.Types;

internal class BigInt : Type
{
    public BigInt() : base("BigInt", Any)
    {
    }

    public override MemberDefinition[] DeclaredMembers => new MemberDefinition[]
    {
        new FunctionDefinition
        {
            Name = "asIntN",
            Type = this,
            Parameters =
            {
                new FunctionParameter("bits", Number),
                new FunctionParameter("int", this)
            },
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "asUintN",
            Type = this,
            Parameters =
            {
                new FunctionParameter("bits", Number),
                new FunctionParameter("int", this)
            },
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = FunctionDefinition.ConstructorName,
            Type = this,
            Parameters =
            {
                new FunctionParameter("value", String | Number | this | Boolean)
            },
            DeclaringType = this
        }
    };
}
