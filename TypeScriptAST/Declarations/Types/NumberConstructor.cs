namespace TypeScriptAST.Declarations.Types;

internal class NumberConstructor : Constructor
{
    public NumberConstructor() : base("Number")
    {
    }

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
        new PropertyDefinition
        {
            Name = "EPSILON",
            Type = Number,
            IsStatic = true,
            ReadOnly = true,
            DeclaringType = this
        },
        new PropertyDefinition
        {
            Name = "MAX_VALUE",
            Type = Number,
            IsStatic = true,
            ReadOnly = true,
            DeclaringType = this
        },
        new PropertyDefinition
        {
            Name = "MIN_VALUE",
            Type = Number,
            IsStatic = true,
            ReadOnly = true,
            DeclaringType = this
        },
        new PropertyDefinition
        {
            Name = "MAX_SAFE_INTEGER",
            Type = Number,
            IsStatic = true,
            ReadOnly = true,
            DeclaringType = this
        },
        new PropertyDefinition
        {
            Name = "MIN_SAFE_INTEGER",
            Type = Number,
            IsStatic = true,
            ReadOnly = true,
            DeclaringType = this
        },
        new PropertyDefinition
        {
            Name = "NEGATIVE_INFINITY",
            Type = Number,
            IsStatic = true,
            ReadOnly = true,
            DeclaringType = this
        },
        new PropertyDefinition
        {
            Name = "POSITIVE_INFINITY",
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
        },
        new FunctionDefinition
        {
            Name = "isFinite",
            Type = Boolean,
            Parameters =
            {
                new FunctionParameter("number", Unknown)
            },
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "isNaN",
            Type = Boolean,
            Parameters =
            {
                new FunctionParameter("number", Unknown)
            },
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "isSafeInteger",
            Type = Boolean,
            Parameters =
            {
                new FunctionParameter("number", Unknown)
            },
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "parseFloat",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("string", String)
            },
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "parseInt",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("string", String),
                new FunctionParameter("radix", Number, isOptional: true)
            },
            IsStatic = true,
            DeclaringType = this
        }
    };
}
