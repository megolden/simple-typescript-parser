namespace TypeScriptAST.Declarations.Types;

internal class ObjectConstructor : Constructor
{
    public ObjectConstructor() : base("Object")
    {
    }

    public override MemberDefinition[] DeclaredMembers => new MemberDefinition[]
    {
        new FunctionDefinition
        {
            Name = FunctionDefinition.ConstructorName,
            Type = Object,
            Parameters =
            {
                new FunctionParameter("value", Any, isOptional: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "assign",
            Type = Any,
            Parameters =
            {
                new FunctionParameter("target", Object),
                new FunctionParameter("sources", ArrayOf(Any), isRest: true)
            },
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "freeze",
            Type = Any,
            Parameters =
            {
                new FunctionParameter("o", Any)
            },
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "isExtensible",
            Type = Boolean,
            Parameters =
            {
                new FunctionParameter("o", Any)
            },
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "isFrozen",
            Type = Boolean,
            Parameters =
            {
                new FunctionParameter("o", Any)
            },
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "isSealed",
            Type = Boolean,
            Parameters =
            {
                new FunctionParameter("o", Any)
            },
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "keys",
            Type = ArrayOf(String),
            Parameters =
            {
                new FunctionParameter("o", Object)
            },
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "preventExtensions",
            Type = Unknown,
            Parameters =
            {
                new FunctionParameter("o", Unknown)
            },
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "seal",
            Type = Unknown,
            Parameters =
            {
                new FunctionParameter("o", Unknown)
            },
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "values",
            Type = ArrayOf(Any),
            Parameters =
            {
                new FunctionParameter("o", Object)
            },
            IsStatic = true,
            DeclaringType = this
        }
    };
}
