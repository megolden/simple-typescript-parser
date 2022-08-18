namespace TypeScriptAST.Declarations.Types;

internal class ArrayConstructor : Constructor
{
    public ArrayConstructor() : base("Array")
    {
    }

    public override MemberDefinition[] DeclaredMembers => new MemberDefinition[]
    {
        new FunctionDefinition
        {
            Name = FunctionDefinition.ConstructorName,
            Type = ArrayOf(Any),
            Parameters =
            {
                new FunctionParameter("arrayLength", Number, isOptional: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = FunctionDefinition.ConstructorName,
            Type = ArrayOf(Any),
            Parameters =
            {
                new FunctionParameter("items", ArrayOf(Any), isRest: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "isArray",
            Type = Boolean,
            Parameters =
            {
                new FunctionParameter("arg", Any)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "of",
            Type = ArrayOf(Unknown),
            Parameters =
            {
                new FunctionParameter("items", ArrayOf(Unknown), isRest: true)
            },
            DeclaringType = this
        }
    };
}
