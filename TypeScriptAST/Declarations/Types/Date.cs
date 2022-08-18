namespace TypeScriptAST.Declarations.Types;

internal class Date : Type
{
    public Date() : base("Date", Object)
    {
    }

    public override MemberDefinition[] DeclaredMembers => new MemberDefinition[]
    {
        new FunctionDefinition
        {
            Name = FunctionDefinition.ConstructorName,
            Type = this,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = FunctionDefinition.ConstructorName,
            Type = this,
            Parameters =
            {
                new FunctionParameter("value", String | Number | Date)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = FunctionDefinition.ConstructorName,
            Type = this,
            Parameters =
            {
                new FunctionParameter("value", String | Number)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = FunctionDefinition.ConstructorName,
            Type = this,
            Parameters =
            {
                new FunctionParameter("year", Number),
                new FunctionParameter("month", Number),
                new FunctionParameter("date", Number, isOptional: true),
                new FunctionParameter("hours", Number, isOptional: true),
                new FunctionParameter("minutes", Number, isOptional: true),
                new FunctionParameter("seconds", Number, isOptional: true),
                new FunctionParameter("ms", Number, isOptional: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "UTC",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("year", Number),
                new FunctionParameter("month", Number),
                new FunctionParameter("date", Number, isOptional: true),
                new FunctionParameter("hours", Number, isOptional: true),
                new FunctionParameter("minutes", Number, isOptional: true),
                new FunctionParameter("seconds", Number, isOptional: true),
                new FunctionParameter("ms", Number, isOptional: true)
            },
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "now",
            Type = Number,
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "parse",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("s", String)
            },
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "getDate",
            Type = Number,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "getDay",
            Type = Number,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "getFullYear",
            Type = Number,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "getHours",
            Type = Number,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "getMilliseconds",
            Type = Number,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "getMinutes",
            Type = Number,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "getMonth",
            Type = Number,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "getSeconds",
            Type = Number,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "getTime",
            Type = Number,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "getTimezoneOffset",
            Type = Number,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "getUTCDate",
            Type = Number,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "getUTCDay",
            Type = Number,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "getUTCFullYear",
            Type = Number,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "getUTCHours",
            Type = Number,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "getUTCMilliseconds",
            Type = Number,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "getUTCMinutes",
            Type = Number,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "getUTCMonth",
            Type = Number,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "getUTCSeconds",
            Type = Number,
            DeclaringType = this
        }
    };
}
