using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Declarations;

public class ConstDeclaration : ModuleDeclaration
{
    public ConstDeclaration(string fullName) : this(fullName, Type.Any)
    {
    }
    public ConstDeclaration(string fullName, Type type) : base(fullName, type)
    {
    }

    public override string ToString()
    {
        return $"{FullName}: {Type}";
    }
}
