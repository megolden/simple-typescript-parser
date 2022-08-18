namespace TypeScriptAST.Declarations.Types;

internal class String : Type
{
    public String() : base("string", Any) { }

    public override MemberDefinition[] DeclaredMembers => new MemberDefinition[]
    {
        new FunctionDefinition
        {
            Name = "charAt",
            Type = String,
            Parameters =
            {
                new FunctionParameter("pos", Number)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "charCodeAt",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("index", Number)
            },
            DeclaringType = this
        },
        new PropertyDefinition
        {
            Name = "length",
            Type = Number,
            ReadOnly = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "trim",
            Type = String,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "trimStart",
            Type = String,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "trimEnd",
            Type = String,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "toString",
            Type = String,
            DeclaringType = this
        }
    };
}
