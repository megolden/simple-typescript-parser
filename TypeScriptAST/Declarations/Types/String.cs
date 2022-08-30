namespace TypeScriptAST.Declarations.Types;

internal class String : Type
{
    public String() : base("string", Any)
    {
    }

    public override MemberDefinition[] DeclaredMembers => new MemberDefinition[]
    {
        new PropertyDefinition
        {
            Name = "length",
            Type = Number,
            ReadOnly = true,
            DeclaringType = this
        },
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
        new FunctionDefinition
        {
            Name = "codePointAt",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("pos", Number)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "concat",
            Type = String,
            Parameters =
            {
                new FunctionParameter("strings", ArrayOf(String), isRest: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "startsWith",
            Type = Boolean,
            Parameters =
            {
                new FunctionParameter("searchString", String),
                new FunctionParameter("position", Number, isOptional: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "endsWith",
            Type = Boolean,
            Parameters =
            {
                new FunctionParameter("searchString", String),
                new FunctionParameter("endPosition", Number, isOptional: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "includes",
            Type = Boolean,
            Parameters =
            {
                new FunctionParameter("searchString", String),
                new FunctionParameter("position", Number, isOptional: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "indexOf",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("searchString", String),
                new FunctionParameter("position", Number, isOptional: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "lastIndexOf",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("searchString", String),
                new FunctionParameter("position", Number, isOptional: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "localeCompare",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("that", String),
                new FunctionParameter("locales", String | ArrayOf(String), isOptional: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "normalize",
            Type = String,
            Parameters =
            {
                new FunctionParameter("form", String, isOptional: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "padEnd",
            Type = String,
            Parameters =
            {
                new FunctionParameter("maxLength", Number),
                new FunctionParameter("fillString", String, isOptional: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "padStart",
            Type = String,
            Parameters =
            {
                new FunctionParameter("maxLength", Number),
                new FunctionParameter("fillString", String, isOptional: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "repeat",
            Type = String,
            Parameters =
            {
                new FunctionParameter("count", Number)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "replace",
            Type = String,
            Parameters =
            {
                new FunctionParameter("searchValue", String | RegExp),
                new FunctionParameter("replaceValue", String)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "replace",
            Type = String,
            Parameters =
            {
                new FunctionParameter("searchValue", String | RegExp),
                new FunctionParameter(
                    "replacer",
                    FunctionOf(String,
                        ("substring", String, IsOptional: false, IsRest: false),
                        ("args", ArrayOf(Any), IsOptional: false, IsRest: true)))
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "replaceAll",
            Type = String,
            Parameters =
            {
                new FunctionParameter("searchValue", String | RegExp),
                new FunctionParameter("replaceValue", String)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "replaceAll",
            Type = String,
            Parameters =
            {
                new FunctionParameter("searchValue", String | RegExp),
                new FunctionParameter(
                    "replacer",
                    FunctionOf(String,
                        ("substring", String, IsOptional: false, IsRest: false),
                        ("args", ArrayOf(Any), IsOptional: false, IsRest: true)))
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "search",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("regexp", String | RegExp)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "slice",
            Type = String,
            Parameters =
            {
                new FunctionParameter("start", Number, isOptional: true),
                new FunctionParameter("end", Number, isOptional: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "split",
            Type = ArrayOf(String),
            Parameters =
            {
                new FunctionParameter("separator", String | RegExp),
                new FunctionParameter("limit", Number, isOptional: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "substring",
            Type = String,
            Parameters =
            {
                new FunctionParameter("start", Number),
                new FunctionParameter("end", Number, isOptional: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "toLocaleLowerCase",
            Type = String,
            Parameters =
            {
                new FunctionParameter("locales", String | ArrayOf(String), isOptional: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "toLocaleUpperCase",
            Type = String,
            Parameters =
            {
                new FunctionParameter("locales", String | ArrayOf(String), isOptional: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "toLowerCase",
            Type = String,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "toUpperCase",
            Type = String,
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
            Name = "valueOf",
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
