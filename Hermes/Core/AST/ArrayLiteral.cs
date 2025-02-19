namespace Hermes.Core.AST;

internal class ArrayLiteral: ASTNode
{
    public List<ASTNode> Elements { get; }
    public ArrayLiteral(List<ASTNode> elements)
    {
        Elements = elements;
    }

    public override string ToString()
    {
        return $"[{string.Join(", ", Elements)}]";
    }
}