namespace TypeScriptAST.Declarations.Types;

internal abstract class Constructor : Type
{
    protected Constructor(string fullName) : base(fullName, Object) { }
}
