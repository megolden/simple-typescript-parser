using System;

namespace TypeScriptAST;

[Serializable]
public sealed class Undefined :
    IComparable<Undefined>,
    IComparable,
    IEquatable<Undefined>
{
    private const int HashCode = 1;

    public static readonly Undefined Value = new();

    private Undefined() { }

    public override string ToString()
    {
        return String.Empty;
    }

    public int CompareTo(object? obj)
    {
        return obj is Undefined ? 0 : -1;
    }

    public int CompareTo(Undefined? other)
    {
        return other is not null ? 0 : -1;
    }

    public bool Equals(Undefined? other)
    {
        return CompareTo(other) is 0;
    }

    public override bool Equals(object? obj)
    {
        return CompareTo(obj) is 0;
    }

    public override int GetHashCode()
    {
        return HashCode;
    }

    public static bool operator ==(Undefined? op1, Undefined? op2)
    {
        return Object.Equals(op1, op2);
    }

    public static bool operator !=(Undefined? op1, Undefined? op2)
    {
        return !(op1 == op2);
    }
}
