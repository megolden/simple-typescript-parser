using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using TypeScriptAST.Declarations.Types;
using Object = System.Object;

namespace TypeScriptAST.Expressions;

public class TemplateLiteral : Expression
{
    public IReadOnlyList<TemplateElement> Elements { get; }

    internal TemplateLiteral(IEnumerable<TemplateElement> elements) : base(Declarations.Types.Type.String)
    {
        Elements = elements.ToList();
    }

    public override string ToString()
    {
        var buffer = new StringBuilder();
        buffer.Append('`');
        foreach (var element in Elements)
        {
            if (element.IsValue(out var value))
            {
                buffer.Append(value);
            }
            if (element.IsExpression(out var expression))
            {
                buffer.Append("${");
                buffer.Append(expression);
                buffer.Append('}');
            }
        }
        buffer.Append('`');
        return buffer.ToString();
    }

    [Serializable]
    public sealed class TemplateElement
    {
        private readonly string? _value;
        private readonly Expression? _expression;

        public string Value => _value ?? throw new InvalidOperationException();
        public Expression Expression => _expression ?? throw new InvalidOperationException();

        internal TemplateElement(string value)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            _value = value;
            _expression = null;
        }

        internal TemplateElement(Expression expression)
        {
            if (expression is null)
                throw new ArgumentNullException(nameof(expression));

            _expression = expression;
            _value = null;
        }

        public bool IsValue() => IsValue(out _);
        public bool IsValue([MaybeNullWhen(false)] out string value)
        {
            if (_value is not null)
            {
                value = _value;
                return true;
            }
            value = default;
            return false;
        }

        public bool IsExpression() => IsExpression(out _);
        public bool IsExpression([MaybeNullWhen(false)] out Expression expression)
        {
            if (_expression is not null)
            {
                expression = _expression;
                return true;
            }
            expression = default;
            return false;
        }

        public override string ToString()
        {
            return ((object)_value ?? _expression).ToString();
        }

        public override bool Equals(object? obj)
        {
            return obj is TemplateElement that &&
                   Object.Equals(this._value, that._value) &&
                   Object.Equals(this._expression, that._expression);
        }

        public override int GetHashCode()
        {
            return ((object)_value ?? _expression).GetHashCode();
        }

        public static implicit operator TemplateElement(string value)
        {
            return new TemplateElement(value);
        }

        public static implicit operator TemplateElement(Expression expression)
        {
            return new TemplateElement(expression);
        }
    }
}
