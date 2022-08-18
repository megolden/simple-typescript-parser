namespace TypeScriptAST.Declarations;

public class PropertyDefinition : MemberDefinition
{
    public bool HasGetter { get; set; }
    public bool HasSetter { get; set; }
    public bool IsOptional { get; set; }
    public bool ReadOnly { get; set; }
    public object? InitialValue { get; set; }

    public PropertyDefinition()
    {
        Type = Types.Type.Any;
    }

    public override string ToString()
    {
        return $"{Name}{(IsOptional ? "?" : "")}: {Type}" +
               (InitialValue is not null ? $" = {InitialValue}" : "");
    }
}
