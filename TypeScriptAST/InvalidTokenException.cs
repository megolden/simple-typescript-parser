using System;

namespace TypeScriptAST;

public class InvalidTokenException : Exception
{
    public Token Token { get; }

    public InvalidTokenException(Token token) :
        base($"Invalid token at position {token.Position}: '{token.Text}'")
    {
        Token = token;
    }
}
