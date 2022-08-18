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
        Constants.Add(new ConstDeclaration
        {
            Name = "console",
            Type = Type.Console
        });
    }

    private void AddGlobalFunctions()
    {
        Functions.Add(new FunctionDeclaration
        {
            Name = "isNaN",
            Type = Type.Boolean,
            Parameters =
            {
                new FunctionParameter("number", Type.Number)
            }
        });

        Functions.Add(new FunctionDeclaration
        {
            Name = "isFinite",
            Type = Type.Boolean,
            Parameters =
            {
                new FunctionParameter("number", Type.Number)
            }
        });

        Functions.Add(new FunctionDeclaration
        {
            Name = "eval",
            Type = Type.Any,
            Parameters =
            {
                new FunctionParameter("x", Type.String)
            }
        });

        Functions.Add(new FunctionDeclaration
        {
            Name = "parseFloat",
            Type = Type.Number,
            Parameters =
            {
                new FunctionParameter("string", Type.String)
            }
        });

        Functions.Add(new FunctionDeclaration
        {
            Name = "parseInt",
            Type = Type.Number,
            Parameters =
            {
                new FunctionParameter("string", Type.String),
                new FunctionParameter("radix", Type.Number, isOptional: true)
            }
        });

        Functions.Add(new FunctionDeclaration
        {
            Name = "encodeURI",
            Type = Type.String,
            Parameters =
            {
                new FunctionParameter("uri", Type.String)
            }
        });

        Functions.Add(new FunctionDeclaration
        {
            Name = "encodeURIComponent",
            Type = Type.String,
            Parameters =
            {
                new FunctionParameter("uriComponent", Type.String | Type.Number | Type.Boolean)
            }
        });

        Functions.Add(new FunctionDeclaration
        {
            Name = "decodeURI",
            Type = Type.String,
            Parameters =
            {
                new FunctionParameter("encodedURI", Type.String)
            }
        });

        Functions.Add(new FunctionDeclaration
        {
            Name = "decodeURIComponent",
            Type = Type.String,
            Parameters =
            {
                new FunctionParameter("encodedURIComponent", Type.String)
            }
        });

        Functions.Add(new FunctionDeclaration
        {
            Name = "Date",
            Type = Type.String
        });
    }

    private void AddBuiltInTypes()
    {
        Types.AddRange(TypeResolver.BuiltInTypes);
    }

    private void AddCommonTypes()
    {
        Types.Add(Type.RegExp);
        Types.Add(Type.Date);
        Types.Add(Type.Console);
    }
}
