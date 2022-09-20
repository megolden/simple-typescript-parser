using System;

namespace TypeScriptAST;

public class InvalidTokenException : Exception
{
    public Token Token { get; }

    public InvalidTokenException(string message, Token token, Exception? innerException = null)
        : base(message, innerException)
    {
        Token = token;
    }

    public InvalidTokenException(Token token, Exception? innerException = null)
        : this($"Invalid token at position {token.Position}: '{token.Text}'", token, innerException)
    {
    }
}
