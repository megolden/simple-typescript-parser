using System;
using System.Collections.Generic;
using System.Linq;
using String = System.String;
using Type = TypeScriptAST.Declarations.Types.Type;

namespace TypeScriptAST.Declarations;

public class FunctionMember : MemberDefinition
{
    public const string ConstructorName = "constructor";
    private const string AltConstructorName = "new";

    public IReadOnlyList<FunctionParameter> Parameters { get; }

    public override string FullName => HasNoName ? base.FullName.TrimEnd('.') : base.FullName;
    public bool IsConstructor => Name is ConstructorName or AltConstructorName;
    public bool IsClassConstructor => Name is ConstructorName;
    public bool HasNoName => Name.Length == 0;

    public FunctionMember(
        string name,
        bool isStatic = false,
        MemberModifier modifier = MemberModifier.Public)
        : this(name, Type.Void, Array.Empty<FunctionParameter>(), isStatic, modifier)
    {
    }
    public FunctionMember(
        string name,
        Type type,
        bool isStatic = false,
        MemberModifier modifier = MemberModifier.Public)
        : this(name, type, Array.Empty<FunctionParameter>(), isStatic, modifier)
    {
    }
    public FunctionMember(
        string name,
        IList<FunctionParameter> parameters,
        bool isStatic = false,
        MemberModifier modifier = MemberModifier.Public)
        : this(name, Type.Void, parameters, isStatic, modifier)
    {
    }
    public FunctionMember(
        string name,
        Type type,
        IList<FunctionParameter> parameters,
        bool isStatic = false,
        MemberModifier modifier = MemberModifier.Public)
        : base(name, type, isStatic, modifier)
    {
        Parameters = parameters.ToList();
    }

    public override string ToString()
    {
        return $"{Name}({String.Join(", ", Parameters)}): {Type}";
    }

    public override MemberDefinition Clone()
    {
        return new FunctionMember(Name, Type, Parameters.ToArray(), IsStatic, Modifier);
    }
}
