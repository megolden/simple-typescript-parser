using System;
using System.Collections.Generic;
using System.Linq;

namespace TypeScriptAST.Declarations.Types;

public class UnionType : Type
{
    private readonly bool _isAnonymous;

    public IReadOnlyCollection<Type> UnionTypes { get; }

    internal UnionType(IEnumerable<Type> types, Type? ownerType = null)
        : this(GenerateAnonymousName(), types, ownerType)
    {
        _isAnonymous = true;
    }

    internal UnionType(string fullName, IEnumerable<Type> types, Type? ownerType = null)
        : this(fullName, types, typesIsResolved: false, ownerType)
    {
    }

    private UnionType(IEnumerable<Type> types, bool typesIsResolved, Type? ownerType = null)
        : this(GenerateAnonymousName(), types, typesIsResolved, ownerType)
    {
        _isAnonymous = true;
    }

    private UnionType(string fullName, IEnumerable<Type> types, bool typesIsResolved, Type? ownerType = null)
        : base(fullName, ownerType)
    {
        var resolvedTypes = typesIsResolved ? types.ToArray() : ResolveUnionTypes(types);

        if (resolvedTypes.Count < 2)
            throw new ArgumentException("union types must have at least 2 types");

        _isAnonymous = false;
        UnionTypes = resolvedTypes.ToArray();
    }

    public override string ToString()
    {
        return System.String.Join(" | ", UnionTypes.AsEnumerable());
    }

    public override bool IsAssignableFrom(Type type)
    {
        return base.IsAssignableFrom(type) || UnionTypes.Any(_ => _.IsAssignableFrom(type)) ||
               (type is UnionType unionType && unionType.UnionTypes.All(IsAssignableFrom));
    }

    protected override bool EqualTo(Type that)
    {
        if (!_isAnonymous) return base.EqualTo(that);

        return that is UnionType thatUnion && UnionTypes.SequenceEqual(thatUnion.UnionTypes);
    }

    private static IList<Type> ResolveUnionTypes(IEnumerable<Type> types)
    {
        IEnumerable<Type> UnwrapTypes(Type type)
        {
            if (type is UnionType unionType)
                return unionType.UnionTypes.SelectMany(UnwrapTypes);
            return new[] { type };
        }

        return types.SelectMany(UnwrapTypes).Distinct().ToArray();
    }

    public static Type UnionOrTypeOf(IEnumerable<Type> types)
    {
        var resolvedTypes = ResolveUnionTypes(types);
        return resolvedTypes.Count >= 2 ?
            new UnionType(resolvedTypes, typesIsResolved: true) :
            resolvedTypes[0];
    }
}
