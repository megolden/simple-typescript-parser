using System.Collections.Generic;
using System.Linq;
using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Declarations;

public class ModuleDef
{
    private readonly List<Type> _types = new();
    private readonly List<ModuleDeclaration> _declarations = new();

    public IReadOnlyCollection<Type> Types => _types.ToArray();
    public IReadOnlyCollection<ModuleDeclaration> Declarations => _declarations.ToArray();

    public IEnumerable<VarDeclaration> Vars => _declarations.OfType<VarDeclaration>();
    public IEnumerable<LetDeclaration> Lets => _declarations.OfType<LetDeclaration>();
    public IEnumerable<ConstDeclaration> Constants => _declarations.OfType<ConstDeclaration>();
    public IEnumerable<FunctionDeclaration> Functions => _declarations.OfType<FunctionDeclaration>();

    public void Add(params Type[] types)
    {
        _types.AddRange(types);
    }

    public void Add(params ModuleDeclaration[] declarations)
    {
        _declarations.AddRange(declarations);
    }
}
