namespace TypeScriptAST;

public class EmptyStatement : Statement
{
    internal EmptyStatement() { }

    public override string ToString()
    {
        return ";";
    }
}
