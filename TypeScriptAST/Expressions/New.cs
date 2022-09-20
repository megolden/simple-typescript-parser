using System;
using System.Collections.Generic;
using System.Linq;
using TypeScriptAST.Declarations;

namespace TypeScriptAST.Expressions;

public class New : Operator
{
    public FunctionMember Constructor { get; private init; }
    public IReadOnlyList<Expression> Arguments { get; private init; }

    internal New(FunctionMember constructor, IEnumerable<Expression> arguments)
        : base(OperatorType.New, "new", constructor.Type)
    {
        Constructor = constructor;
        Arguments = arguments.ToList();
    }

    public override string ToString()
    {
        return $"new {Constructor.DeclaringType}({String.Join(", ", Arguments)})";
    }
}
