using System;
using System.Collections.Generic;
using System.Linq;

namespace TypeScriptAST.Declarations.Types;

internal class Function : Type
{
    private readonly IEnumerable<Parameter> _parameters;

    public Type Type { get; private init; }
    public Parameter[] Parameters => _parameters.ToArray();

    public Function(IEnumerable<Parameter> parameters, Type type) : base(System.String.Empty, Object)
    {
        _parameters = parameters;
        Type = type;
    }

    public override string ToString()
    {
        return $"({System.String.Join(", ", _parameters)}) => {Type}";
    }

    public override bool IsAssignableFrom(Type type)
    {
        return base.IsAssignableFrom(type) || (type is Function that && EqualSignature(this, that));
    }

    private bool EqualSignature(Function fn1, Function fn2)
    {
        return fn1.Type == fn2.Type && fn1._parameters.SequenceEqual(fn2._parameters);
    }

    public class Parameter
    {
        public string Name { get; private init; }
        public Type Type { get; private init; }
        public bool IsOptional { get; private init; }
        public bool IsRest { get; private init; }

        public Parameter(string name, Type type, bool isOptional = false, bool isRest = false)
        {
            Name = name;
            Type = type;
            IsOptional = isOptional;
            IsRest = isRest;
        }

        public override string ToString()
        {
            return $"{(IsRest ? "..." : "")}{Name}{(IsOptional && !IsRest ? "?" : "")}: {Type}";
        }

        public override bool Equals(object? obj)
        {
            return obj is Parameter that &&
                   this.Name == that.Name &&
                   this.Type == that.Type &&
                   this.IsOptional == that.IsOptional &&
                   this.IsRest == that.IsRest;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Type, IsOptional, IsRest);
        }
    }
}
