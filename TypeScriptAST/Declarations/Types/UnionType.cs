using System;
using System.Collections.Generic;
using System.Linq;

namespace TypeScriptAST.Declarations.Types;

internal class UnionType : Type
{
    public override bool IsUnion => true;
    public override Type[] UnionTypes { get; }

    public UnionType(IEnumerable<Type> types) : this(System.String.Empty, types)
    {
    }

    public UnionType(string fullName, IEnumerable<Type> types) : base(fullName, Object)
    {
        IEnumerable<Type> ResolveTypes(Type type)
        {
            if (type is AliasType { IsUnion: true } aliasType)
                return aliasType.UnionTypes.SelectMany(ResolveTypes);
            return new[] { type };
        }

        var resolvedTypes = types.SelectMany(ResolveTypes).Distinct().ToArray();

        if (resolvedTypes.Length < 2)
            throw new ArgumentException("union types must have at least 2 types");

        UnionTypes = resolvedTypes;
    }

    public override string ToString()
    {
        return System.String.Join(" | ", UnionTypes.AsEnumerable());
    }
}
