namespace TypeScriptAST.Declarations.Types;

internal class RegExp : ClassType
{
    public RegExp() : base("RegExp", isAbstract: false)
    {
    }

    public override MemberDefinition[] DeclaredMembers => new MemberDefinition[]
    {
        new PropertyDefinition
        {
            Name = "dotAll",
            Type = Boolean,
            ReadOnly = true,
            DeclaringType = this
        },
        new PropertyDefinition
        {
            Name = "flags",
            Type = String,
            ReadOnly = true,
            DeclaringType = this
        },
        new PropertyDefinition
        {
            Name = "global",
            Type = Boolean,
            ReadOnly = true,
            DeclaringType = this
        },
        new PropertyDefinition
        {
            Name = "ignoreCase",
            Type = Boolean,
            ReadOnly = true,
            DeclaringType = this
        },
        new PropertyDefinition
        {
            Name = "lastIndex",
            Type = Number,
            ReadOnly = true,
            DeclaringType = this
        },
        new PropertyDefinition
        {
            Name = "multiline",
            Type = Boolean,
            ReadOnly = true,
            DeclaringType = this
        },
        new PropertyDefinition
        {
            Name = "source",
            Type = String,
            ReadOnly = true,
            DeclaringType = this
        },
        new PropertyDefinition
        {
            Name = "sticky",
            Type = Boolean,
            ReadOnly = true,
            DeclaringType = this
        },
        new PropertyDefinition
        {
            Name = "unicode",
            Type = Boolean,
            ReadOnly = true,
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = FunctionDefinition.ConstructorName,
            Type = this,
            Parameters =
            {
                new FunctionParameter("pattern", String | RegExp),
                new FunctionParameter("flags", String, isOptional: true)
            },
            DeclaringType = this
        },
        new FunctionDefinition
        {
            Name = "test",
            Type = Boolean,
            Parameters =
            {
                new FunctionParameter("string", String)
            },
            DeclaringType = this
        }
    };
}
