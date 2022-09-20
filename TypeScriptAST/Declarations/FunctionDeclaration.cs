using System;
using System.Collections.Generic;
using System.Linq;
using String = System.String;
using Type = TypeScriptAST.Declarations.Types.Type;

namespace TypeScriptAST.Declarations;

public class FunctionDeclaration : ModuleDeclaration
{
    public IReadOnlyList<FunctionParameter> Parameters { get; }

    public FunctionDeclaration(string fullName)
        : this(fullName, Array.Empty<FunctionParameter>(), Type.Void)
    {
    }
    public FunctionDeclaration(string fullName, Type type)
        : this(fullName, Array.Empty<FunctionParameter>(), type)
    {
    }
    public FunctionDeclaration(string fullName, IList<FunctionParameter> parameters)
        : this(fullName, parameters, Type.Void)
    {
    }
    public FunctionDeclaration(string fullName, IList<FunctionParameter> parameters, Type type)
        : base(fullName, type)
    {
        Parameters = parameters.ToList();
    }

    public override string ToString()
    {
        return $"{FullName}({String.Join(", ", Parameters)}): {Type}";
    }
}
