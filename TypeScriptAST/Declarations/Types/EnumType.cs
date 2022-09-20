using System;
using System.Collections.Generic;
using System.Linq;

namespace TypeScriptAST.Declarations.Types;

public class EnumType : Type
{
    public Type ValueType { get; }
    public IReadOnlyCollection<MemberDefinition> Members { get; }

    internal EnumType(string fullName, IDictionary<string, object> values, Type? ownerType = null)
        : base(fullName, ownerType)
    {
        if (!values.Any())
            throw new InvalidOperationException($"enums must have at least on value: {fullName}");

        ValueType = IsString(values.Values.First())  ? String :
                    IsNumeric(values.Values.First()) ? Number :
                    throw new NotSupportedException("enum values must be string or number");

        if ((ValueType == String && !values.Values.All(IsString)) ||
            (ValueType == Number && !values.Values.All(IsNumeric)))
            throw new InvalidOperationException($"enum type values must be only number or string: {fullName}");

        Members = GenerateMembers(values).ToList();
    }

    private IEnumerable<MemberDefinition> GenerateMembers(IDictionary<string, object> values)
    {
        return values.SelectMany(item => new[]
        {
            new PropertyMember(
                item.Key,
                ValueType,
                readOnly: true,
                isStatic: true,
                initialValue: item.Value)
                .WithDeclaringType(this),

            new PropertyMember(
                item.Value.ToString(),
                String,
                readOnly: true,
                isStatic: true,
                initialValue: item.Key)
                .WithDeclaringType(this)
        });
    }

    private bool IsString(object value)
    {
        return value is string;
    }

    private bool IsNumeric(object value)
    {
        return value is
            byte or sbyte or
            short or ushort or
            int or uint or
            long or ulong or
            float or double;
    }

    public override bool IsAssignableFrom(Type type)
    {
        return base.IsAssignableFrom(type) ||
               ValueType.IsAssignableFrom(type);
    }
}
