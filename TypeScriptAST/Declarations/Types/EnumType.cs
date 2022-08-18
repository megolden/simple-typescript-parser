using System;
using System.Collections.Generic;
using System.Linq;

namespace TypeScriptAST.Declarations.Types;

internal class EnumType : Type
{
    public sealed override bool IsEnum => true;
    public sealed override Type UnderlyingType { get; }
    public override MemberDefinition[] DeclaredMembers { get; }

    public EnumType(string fullName, IDictionary<string, object> values) : base(fullName, Object)
    {
        if (!values.Any())
            throw new InvalidOperationException($"enums must have at least on value: {fullName}");

        UnderlyingType = IsString(values.Values.First()) ? String : Number;

        if ((UnderlyingType is String && values.Any(_ => !IsString(_.Value))) ||
            (UnderlyingType is Number && values.Any(_ => !IsNumeric(_.Value))))
            throw new InvalidOperationException($"enum type values must be only number or string: {fullName}");

        DeclaredMembers = GetMembers(values).ToArray();
    }

    private IEnumerable<MemberDefinition> GetMembers(IDictionary<string, object> values)
    {
        IEnumerable<MemberDefinition> ValueToMembers(KeyValuePair<string, object> item)
        {
            yield return new PropertyDefinition
            {
                Name = item.Key,
                Type = UnderlyingType,
                InitialValue = item.Value,
                ReadOnly = true,
                IsStatic = true,
                DeclaringType = this
            };
            yield return new PropertyDefinition
            {
                Name = item.Value.ToString(),
                Type = String,
                InitialValue = item.Key,
                ReadOnly = true,
                IsStatic = true,
                DeclaringType = this
            };
        }

        foreach (var member in values.SelectMany(ValueToMembers))
            yield return member;
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
}
