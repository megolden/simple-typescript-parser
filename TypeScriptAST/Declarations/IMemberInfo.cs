using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Declarations;

public interface IMemberInfo
{
    string Name { get; }
    Type Type { get; }
    string FullName { get; }
}