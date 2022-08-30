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
        new PropertyDefinition
        {
            Name = "LN10",
            Type = Number,
            IsStatic = true,
            ReadOnly = true,
            DeclaringType = this
        },
        new PropertyDefinition
        {
            Name = "LN2",
            Type = Number,
            IsStatic = true,
            ReadOnly = true,
            DeclaringType = this
        },
        new PropertyDefinition
        {
            Name = "LOG10E",
            Type = Number,
            IsStatic = true,
            ReadOnly = true,
            DeclaringType = this
        },
        new PropertyDefinition
        {
            Name = "LOG2E",
            Type = Number,
            IsStatic = true,
            ReadOnly = true,
            DeclaringType = this
        },
        new PropertyDefinition
        {
            Name = "PI",
            Type = Number,
            InitialValue = System.Math.PI,
            IsStatic = true,
            ReadOnly = true,
            DeclaringType = this
        },
        new PropertyDefinition
        {
            Name = "SQRT1_2",
            Type = Number,
            IsStatic = true,
            ReadOnly = true,
            DeclaringType = this
        },
        new PropertyDefinition
        {
            Name = "SQRT2",
            Type = Number,
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
        },
        new FunctionDefinition
        {
            Name = "abs",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("x", Number)
            },
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "acos",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("x", Number)
            },
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "acosh",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("x", Number)
            },
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "asin",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("x", Number)
            },
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "asinh",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("x", Number)
            },
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "atan",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("x", Number)
            },
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "atan2",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("y", Number),
                new FunctionParameter("x", Number)
            },
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "atanh",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("x", Number)
            },
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "cbrt",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("x", Number)
            },
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "ceil",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("x", Number)
            },
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "clz32",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("x", Number)
            },
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "cos",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("x", Number)
            },
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "cosh",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("x", Number)
            },
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "exp",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("x", Number)
            },
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "expm1",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("x", Number)
            },
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "fround",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("x", Number)
            },
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "hypot",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("values", ArrayOf(Number), isRest: true)
            },
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "imul",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("x", Number),
                new FunctionParameter("y", Number)
            },
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "log",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("x", Number)
            },
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "log10",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("x", Number)
            },
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "log1p",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("x", Number)
            },
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "log2",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("x", Number)
            },
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "max",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("values", ArrayOf(Number), isRest: true)
            },
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "min",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("values", ArrayOf(Number), isRest: true)
            },
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "pow",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("x", Number),
                new FunctionParameter("y", Number)
            },
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "random",
            Type = Number,
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "round",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("x", Number)
            },
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "sign",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("x", Number)
            },
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "sin",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("x", Number)
            },
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "sinh",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("x", Number)
            },
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "sqrt",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("x", Number)
            },
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "tan",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("x", Number)
            },
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "tanh",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("x", Number)
            },
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "trunc",
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
