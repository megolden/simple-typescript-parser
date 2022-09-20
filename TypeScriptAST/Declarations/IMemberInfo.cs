using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Declarations;

public interface IMemberInfo
{
    string Name { get; }
    string FullName { get; }
    Type Type { get; }
}
