using System.Collections.Generic;
using System.Linq;
using Type = TypeScriptAST.Declarations.Types.Type;

namespace TypeScriptAST.Expressions;

public class Array : Expression
{
    public Type ElementType { get; }
    public IReadOnlyList<Expression> Items { get; }

    internal Array(params Expression[] items) : base(Type.ArrayOf(ResolveElementType(items)))
    {
        Items = items.ToList();
        ElementType = ResolveElementType(items);
    }

    private static Type ResolveElementType(IList<Expression> items)
    {
        return items.Count > 0 && items.All(_ => _.Type == items[0].Type) ?
            items[0].Type :
            Type.Any;
    }

    public override string ToString()
    {
        return $"{ElementType.Name}[]";
    }
}
