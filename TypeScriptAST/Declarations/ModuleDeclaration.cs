using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Declarations;

public abstract class ModuleDeclaration : IMemberInfo
{
    public string Name { get; }
    public string? Namespace { get; }
    public Type Type { get; }

    public string FullName => $"{(Namespace is { Length: > 0 } ns ? ns + "." : "")}{Name}";

    protected ModuleDeclaration(string fullName, Type type)
    {
        var sepIndex = fullName.LastIndexOf('.');
        Name = sepIndex >= 0 ? fullName.Substring(sepIndex + 1) : fullName;
        Namespace = sepIndex >= 0 ? fullName.Remove(sepIndex) : null;
        Type = type;
    }
}
