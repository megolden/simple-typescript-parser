namespace TypeScriptAST.Declarations.Types;

internal class ObjectConstructor : Constructor
{
    public ObjectConstructor() : base("Object") { }

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
        }
    };
}
