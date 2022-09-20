namespace TypeScriptAST.Declarations.Types;

public class AliasType : Type
{
    public Type ReferencedType { get; }

    internal AliasType(string fullName, Type referencedType, Type? ownerType = null) : base(fullName, ownerType)
    {
        ReferencedType = referencedType;
    }

    public override bool IsAssignableFrom(Type type)
    {
        return base.IsAssignableFrom(type) ||
               ReferencedType.IsAssignableFrom(type);
    }
}
