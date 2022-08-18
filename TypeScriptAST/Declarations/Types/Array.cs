namespace TypeScriptAST.Declarations.Types;

internal class Array : Type
{
    public override Type UnderlyingType { get; }
    public override bool IsArray => true;

    public Array(Type elementType) : base($"{elementType.FullName}[]", Object)
    {
        UnderlyingType = elementType;
    }

    public override MemberDefinition[] DeclaredMembers => new MemberDefinition[]
    {
        new PropertyDefinition
        {
            Name = "length",
            Type = Number,
            ReadOnly = true,
            DeclaringType = this
        }
    };

    public override string ToString()
    {
        var underlying = UnderlyingType.ToString();
        if (UnderlyingType.IsUnion)
            underlying = "(" + underlying + ")";
        return $"{underlying}[]";
    }
}
