namespace TypeScriptAST.Declarations.Types;

internal class Console : Type
{
    public Console() : base("Console", Object)
    {
    }

    public override MemberDefinition[] DeclaredMembers => new MemberDefinition[]
    {
        new FunctionDefinition
        {
            Name = "assert",
            Type = Void,
            Parameters =
            {
                new FunctionParameter("condition", Boolean, isOptional: true),
                new FunctionParameter("data", ArrayOf(Any), isRest: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "clear",
            Type = Void,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "count",
            Type = Void,
            Parameters =
            {
                new FunctionParameter("label", String, isOptional: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "countReset",
            Type = Void,
            Parameters =
            {
                new FunctionParameter("label", String, isOptional: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "debug",
            Type = Void,
            Parameters =
            {
                new FunctionParameter("data", ArrayOf(Any), isRest: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "dir",
            Type = Void,
            Parameters =
            {
                new FunctionParameter("item", Any, isOptional: true),
                new FunctionParameter("options", Any, isOptional: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "dirxml",
            Type = Void,
            Parameters =
            {
                new FunctionParameter("data", ArrayOf(Any), isRest: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "error",
            Type = Void,
            Parameters =
            {
                new FunctionParameter("data", ArrayOf(Any), isRest: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "group",
            Type = Void,
            Parameters =
            {
                new FunctionParameter("data", ArrayOf(Any), isRest: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "groupCollapsed",
            Type = Void,
            Parameters =
            {
                new FunctionParameter("data", ArrayOf(Any), isRest: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "groupEnd",
            Type = Void,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "info",
            Type = Void,
            Parameters =
            {
                new FunctionParameter("data", ArrayOf(Any), isRest: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "log",
            Type = Void,
            Parameters =
            {
                new FunctionParameter("data", ArrayOf(Any), isRest: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "table",
            Type = Void,
            Parameters =
            {
                new FunctionParameter("tabularData", Any, isOptional: true),
                new FunctionParameter("properties", ArrayOf(String), isOptional: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "time",
            Type = Void,
            Parameters =
            {
                new FunctionParameter("label", String, isOptional: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "timeEnd",
            Type = Void,
            Parameters =
            {
                new FunctionParameter("label", String, isOptional: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "timeLog",
            Type = Void,
            Parameters =
            {
                new FunctionParameter("label", String, isOptional: true),
                new FunctionParameter("data", ArrayOf(Any), isRest: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "timeStamp",
            Type = Void,
            Parameters =
            {
                new FunctionParameter("label", String, isOptional: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "trace",
            Type = Void,
            Parameters =
            {
                new FunctionParameter("data", ArrayOf(Any), isRest: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "warn",
            Type = Void,
            Parameters =
            {
                new FunctionParameter("data", ArrayOf(Any), isRest: true)
            },
            DeclaringType = this
        }
    };
}
