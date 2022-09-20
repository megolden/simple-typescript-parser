namespace TypeScriptAST;

public class Comment : Statement
{
    public string Text { get; }
    public bool SingleLine { get; }
    public bool MultiLine { get; }

    internal Comment(string text, bool singleLine, bool multiLine)
    {
        Text = text;
        SingleLine = singleLine;
        MultiLine = multiLine;
    }

    public override string ToString()
    {
        return SingleLine
            ? "// " + Text
            : "/* " + Text + " */";
    }
}
