using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TypeScriptAST;

public class StatementList : Statement, IEnumerable<Statement>
{
    private readonly IReadOnlyList<Statement> _statements;

    public int Count => _statements.Count;
    public bool IsEmpty => _statements.Count == 0;

    internal StatementList(IEnumerable<Statement> statements) : this(statements.ToArray())
    {
    }
    internal StatementList(params Statement[] statements)
    {
        _statements = statements.ToArray();
    }

    public IEnumerator<Statement> GetEnumerator()
    {
        return _statements.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public override string ToString()
    {
        return String.Join(Environment.NewLine, _statements);
    }
}
