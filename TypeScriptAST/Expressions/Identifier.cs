using TypeScriptAST.Declarations;

namespace TypeScriptAST.Expressions;

public class Identifier : Expression
{
    public string Name { get; private init; }
    public IMemberInfo Subject { get; private init; }

    internal Identifier(IMemberInfo subject) : base(subject.Type)
    {
        Name = subject.Name;
        Subject = subject;
    }

    public override string ToString()
    {
        return Subject.FullName;
    }
}
