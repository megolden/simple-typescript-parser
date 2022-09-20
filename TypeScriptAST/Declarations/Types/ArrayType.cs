namespace TypeScriptAST.Declarations.Types;

public class ArrayType : Type
{
    public Type ElementType { get; }

    internal ArrayType(Type elementType, Type? ownerType = null) : base(GenerateAnonymousName(), ownerType)
    {
        ElementType = elementType;
    }

    public override string ToString()
    {
        var underlying = ElementType is UnionType ?
            "(" + ElementType + ")" :
            ElementType.ToString();
        return $"{underlying}[]";
    }

    public override bool IsAssignableFrom(Type type)
    {
        return base.IsAssignableFrom(type) ||
               (type is ArrayType arrayType && ElementType.IsAssignableFrom(arrayType.ElementType));
    }

    protected override bool EqualTo(Type that)
    {
        return that is ArrayType thatArray && ElementType == thatArray.ElementType;
    }
}
