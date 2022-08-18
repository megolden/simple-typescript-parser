using System.Collections.Generic;
using System.Linq;
using String = System.String;
using Type = TypeScriptAST.Declarations.Types.Type;

namespace TypeScriptAST.Expressions;

public class Array : Expression
{
    public IReadOnlyList<Expression> Items { get; private init; }

    internal Array(IEnumerable<Expression> items) : base(ResolveType(items))
    {
        Items = items.ToList();
    }

    private static Type ResolveType(IEnumerable<Expression> items)
    {
        var itemsList = items.ToList();
        return itemsList.Count > 0 && itemsList.All(_ => _.Type == itemsList[0].Type) ?
            itemsList[0].Type :
            Type.Any;
    }

    public override string ToString()
    {
        return $"[{String.Join(", ", Items)}]";
    }
}
