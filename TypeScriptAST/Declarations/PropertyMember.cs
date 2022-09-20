using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Declarations;

public class PropertyMember : MemberDefinition
{
    public bool IsOptional { get; }
    public bool ReadOnly { get; }
    public object? InitialValue { get; }

    public PropertyMember(
        string name,
        Type type,
        bool isOptional = false,
        bool readOnly = false,
        object? initialValue = null,
        bool isStatic = false,
        MemberModifier modifier = MemberModifier.Public)
        : base(name, type, isStatic, modifier)
    {
        IsOptional = isOptional;
        ReadOnly = readOnly;
        InitialValue = initialValue;
    }

    public override string ToString()
    {
        return $"{Name}{(IsOptional ? "?" : "")}: {Type}" +
               (InitialValue is not null ? $" = {InitialValue}" : "");
    }

    public override PropertyMember Clone()
    {
        return new PropertyMember(Name, Type, IsOptional, ReadOnly, InitialValue, IsStatic, Modifier);
    }
}
