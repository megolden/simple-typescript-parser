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
        },
        new FunctionDefinition
        {
            Name = "setDate",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("date", Number)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "setFullYear",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("year", Number),
                new FunctionParameter("month", Number, isOptional: true),
                new FunctionParameter("date", Number, isOptional: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "setHours",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("hours", Number),
                new FunctionParameter("min", Number, isOptional: true),
                new FunctionParameter("sec", Number, isOptional: true),
                new FunctionParameter("ms", Number, isOptional: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "setMilliseconds",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("ms", Number)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "setMinutes",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("min", Number),
                new FunctionParameter("sec", Number, isOptional: true),
                new FunctionParameter("ms", Number, isOptional: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "setMonth",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("month", Number),
                new FunctionParameter("date", Number, isOptional: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "setSeconds",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("sec", Number),
                new FunctionParameter("ms", Number, isOptional: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "setTime",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("time", Number)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "setUTCDate",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("date", Number)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "setUTCFullYear",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("year", Number),
                new FunctionParameter("month", Number, isOptional: true),
                new FunctionParameter("date", Number, isOptional: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "setUTCHours",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("hours", Number),
                new FunctionParameter("min", Number, isOptional: true),
                new FunctionParameter("sec", Number, isOptional: true),
                new FunctionParameter("ms", Number, isOptional: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "setUTCMilliseconds",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("ms", Number)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "setUTCMinutes",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("min", Number),
                new FunctionParameter("sec", Number, isOptional: true),
                new FunctionParameter("ms", Number, isOptional: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "setUTCMonth",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("month", Number),
                new FunctionParameter("date", Number, isOptional: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "setUTCSeconds",
            Type = Number,
            Parameters =
            {
                new FunctionParameter("sec", Number),
                new FunctionParameter("ms", Number, isOptional: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "toDateString",
            Type = String,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "toISOString",
            Type = String,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "toJSON",
            Type = String,
            Parameters =
            {
                new FunctionParameter("key", Any, isOptional: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "toTimeString",
            Type = String,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "toUTCString",
            Type = String,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "toString",
            Type = String,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "valueOf",
            Type = Number,
            DeclaringType = this
        }
    };
}
