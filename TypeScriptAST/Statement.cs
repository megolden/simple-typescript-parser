using System.Collections.Generic;
using TypeScriptAST.Declarations;

namespace TypeScriptAST;

public abstract class Statement
{
    public static EmptyStatement Empty()
    {
        return new EmptyStatement();
    }

    public static StatementList List(IEnumerable<Statement> statements)
    {
        return new StatementList(statements);
    }

    public static Comment Comment(string text, bool singleLine, bool multiLine)
    {
        return new Comment(text, singleLine, multiLine);
    }

    public static XmlComment XmlComment(string text, string name, IEnumerable<KeyValuePair<string, string>> properties)
    {
        return new XmlComment(text, name, properties);
    }
}
