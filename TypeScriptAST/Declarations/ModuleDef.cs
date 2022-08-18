using System.Collections.Generic;
using System.Linq;
using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Declarations;

public class ModuleDef
{
    public List<Type> Types { get; set; } = new();
    public List<VarDeclaration> Vars { get; set; } = new();
    public List<LetDeclaration> Lets { get; set; } = new();
    public List<ConstDeclaration> Constants { get; set; } = new();
    public List<FunctionDeclaration> Functions { get; set; } = new();

    public IEnumerable<IMemberInfo> Members
        => Vars.Concat<IMemberInfo>(Lets).Concat(Constants).Concat(Functions);
}
