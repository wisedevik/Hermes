namespace Hermes.Core.AST;

internal class Literal : ASTNode
{
    public object Value { get; }

    public Literal(object value)
    {
        Value = value;
    }
}
