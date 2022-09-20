using System;
using System.Collections.Generic;
using System.Linq;

namespace TypeScriptAST.Declarations.Types;

public class FunctionType : Type
{
    public Type Type { get; }
    public IReadOnlyList<FunctionParameter> Parameters { get; }

    internal FunctionType(IEnumerable<FunctionParameter> parameters, Type type, Type? ownerType = null)
        : base(GenerateAnonymousName(), ownerType)
    {
        Type = type;
        Parameters = parameters.ToList();
    }

    public override string ToString()
    {
        return $"({System.String.Join(", ", Parameters)}) => {Type}";
    }

    public override bool IsAssignableFrom(Type type)
    {
        return base.IsAssignableFrom(type) || (type is FunctionType that && IsAssignableSignature(that));
    }

    private bool IsAssignableSignature(FunctionType that)
    {
        return this.Type.IsAssignableFrom(that.Type) &&
               CheckNumberOfParameters() &&
               CheckTypeOfParameters();

        bool CheckNumberOfParameters()
        {
            var min = Parameters.Count == 0 || Parameters.FirstOrDefault() is { IsRest: true } or { IsOptional : true }
                ? 0
                : Parameters.Count(_ => !_.IsRest && !_.IsOptional);
            var max = Parameters.Any(_ => _.IsRest) ? Int32.MaxValue : Parameters.Count;

            return that.Parameters.Count >= min && that.Parameters.Count <= max;
        }

        bool CheckTypeOfParameters()
        {
            Type ResolveParameterType(int index)
            {
                var restParamIndex = Parameters.ToList().FindIndex(_ => _.IsRest);
                return restParamIndex >= 0 && index >= restParamIndex
                    ? TypeSystem.GetElementType(Parameters[restParamIndex].Type)
                    : Parameters[index].Type;
            }

            return that.Parameters
                .Select((param, index) => (Index: index, Type: param.Type)).ToList()
                .All(param => ResolveParameterType(param.Index).IsAssignableFrom(param.Type));
        }
    }

    protected override bool EqualTo(Type that)
    {
        return that is FunctionType thatFunction &&
               Type == thatFunction.Type &&
               Parameters.SequenceEqual(thatFunction.Parameters);
    }
}
