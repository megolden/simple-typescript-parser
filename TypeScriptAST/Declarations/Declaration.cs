namespace TypeScriptAST.Declarations;

public abstract class Declaration
{
    public string Name { get; set; }
    public string? Namespace { get; set; }
    public string FullName => $"{(Namespace is { Length: > 0 } ns ? ns + "." : "")}{Name}";
}