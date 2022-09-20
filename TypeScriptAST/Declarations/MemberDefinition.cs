using System;
using Type = TypeScriptAST.Declarations.Types.Type;

namespace TypeScriptAST.Declarations;

public abstract class MemberDefinition : IMemberInfo, ICloneable
{
    public string Name { get; }
    public Type Type { get; private set; }
    public bool IsStatic { get; }
    public MemberModifier Modifier { get; }
    public Type? DeclaringType { get; private set; }

    public virtual string FullName => $"{(DeclaringType is not null ? DeclaringType.FullName + "." : "")}{Name}";

    protected MemberDefinition(
        string name,
        Type type,
        bool isStatic = false,
        MemberModifier modifier = MemberModifier.Public)
    {
        Name = name;
        Type = type;
        Modifier = modifier;
        IsStatic = isStatic;
    }

    public MemberDefinition WithDeclaringType(Type type)
    {
        DeclaringType = type;
        return this;
    }

    public MemberDefinition WithType(Type type)
    {
        Type = type;
        return this;
    }

    public abstract MemberDefinition Clone();

    object ICloneable.Clone()
    {
        return Clone();
    }
}

public enum MemberModifier
{
    Public,
    Private,
    Protected
}
