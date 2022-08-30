namespace TypeScriptAST.Declarations.Types;

internal class Number : Type
{
    public Number() : base("number", Any)
    {
    }

    public override MemberDefinition[] DeclaredMembers => new MemberDefinition[]
    {
        new FunctionDefinition
        {
            Name = "toString",
            Type = String,
            Parameters =
            {
                new FunctionParameter("radix", Number, isOptional: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "toExponential",
            Type = String,
            Parameters =
            {
                new FunctionParameter("fractionDigits", Number, isOptional: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "toFixed",
            Type = String,
            Parameters =
            {
                new FunctionParameter("fractionDigits", Number, isOptional: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "toPrecision",
            Type = String,
            Parameters =
            {
                new FunctionParameter("precision", Number, isOptional: true)
            },
            DeclaringType = this
        }
    };
}
