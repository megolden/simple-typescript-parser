namespace TypeScriptAST.Declarations.Types;

internal class Json : Type
{
    public Json() : base("JSON", Object)
    {
    }

    public override MemberDefinition[] DeclaredMembers => new MemberDefinition[]
    {
        new FunctionDefinition
        {
            Name = "stringify",
            Type = String,
            Parameters =
            {
                new FunctionParameter("value", Any),
                new FunctionParameter(
                    "replacer",
                    FunctionOf(
                        Any,
                        ("this", Any, false, false),
                        ("key", String, false, false),
                        ("value", Any, false, false)),
                    isOptional: true),
                new FunctionParameter("space", String | Number, isOptional: true)
            },
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "stringify",
            Type = String,
            Parameters =
            {
                new FunctionParameter("value", Any),
                new FunctionParameter("replacer", ArrayOf(String | Number), isOptional: true),
                new FunctionParameter("space", String | Number, isOptional: true)
            },
            IsStatic = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "parse",
            Type = Any,
            Parameters =
            {
                new FunctionParameter("text", String),
                new FunctionParameter(
                    "reviver",
                    FunctionOf(
                        Any,
                        ("this", Any, false, false),
                        ("key", String, false, false),
                        ("value", Any, false, false)),
                    isOptional: true)
            },
            IsStatic = true,
            DeclaringType = this
        }
    };
}
