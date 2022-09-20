using TypeScriptAST.Declarations.Types;

namespace TypeScriptAST.Declarations;

public class GlobalModule : ModuleDef
{
    public const string Namespace = "globalThis";

    public GlobalModule()
    {
        AddBuiltInTypes();
        AddCommonTypes();
        AddGlobalFunctions();
        AddGlobalConstants();
    }

    private void AddGlobalConstants()
    {
        Add(new VarDeclaration("NaN", Type.Number));
        Add(new VarDeclaration("Infinity", Type.Number));
    }

    private void AddGlobalFunctions()
    {
        Add(new FunctionDeclaration(
            "isNaN",
            new[] { new FunctionParameter("number", Type.Number) },
            Type.Boolean));

        Add(new FunctionDeclaration(
            "isFinite",
            new[] { new FunctionParameter("number", Type.Number) },
            Type.Boolean));

        Add(new FunctionDeclaration(
            "eval",
            new[] { new FunctionParameter("x", Type.String) },
            Type.Any));

        Add(new FunctionDeclaration(
            "parseFloat",
            new[] { new FunctionParameter("string", Type.String) },
            Type.Number));

        Add(new FunctionDeclaration(
            "parseInt",
            new[]
            {
                new FunctionParameter("string", Type.String),
                new FunctionParameter("radix", Type.Number, isOptional: true)
            },
            Type.Number));

        Add(new FunctionDeclaration(
            "encodeURI",
            new[] { new FunctionParameter("uri", Type.String) },
            Type.String));

        Add(new FunctionDeclaration(
            "encodeURIComponent",
            new[] { new FunctionParameter("uriComponent", Type.String | Type.Number | Type.Boolean) },
            Type.String));

        Add(new FunctionDeclaration(
            "decodeURI",
            new[] { new FunctionParameter("encodedURI", Type.String) },
            Type.String));

        Add(new FunctionDeclaration(
            "decodeURIComponent",
            new[] { new FunctionParameter("encodedURIComponent", Type.String) },
            Type.String));
    }

    private void AddBuiltInTypes()
    {
        Add(TypeSystem.BuiltInTypes);
    }

    private void AddCommonTypes()
    {
        Add(Type.RegExp);
        Add(Type.CreateInterface("Date", Type.ObjectType));
        Add(Type.CreateInterface("Console", Type.ObjectType));
    }
}
