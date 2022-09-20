using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Declarations;

public class LetDeclaration : ModuleDeclaration
{
    public LetDeclaration(string fullName) : this(fullName, Type.Any)
    {
    }
    public LetDeclaration(string fullName, Type type) : base(fullName, type)
    {
    }

    public override string ToString()
    {
        return $"{FullName}: {Type}";
    }
}
