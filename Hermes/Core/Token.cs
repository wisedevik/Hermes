namespace Hermes.Core;

internal class Token
{
    public TokenType Type { get; set; }
    public string Value { get; set; }

    public Token(TokenType type, string value = null)
    {
        Type = type;
        Value = value;
    }
}
