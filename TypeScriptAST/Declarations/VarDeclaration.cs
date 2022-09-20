using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Declarations;

public class VarDeclaration : ModuleDeclaration
{
    public VarDeclaration(string fullName) : this(fullName, Type.Any)
    {
    }
    public VarDeclaration(string fullName, Type type) : base(fullName, type)
    {
    }

    public override string ToString()
    {
        return $"{FullName}: {Type}";
    }
}
