namespace Hermes.Core.AST;

internal class BinaryExpression : ASTNode
{
    public ASTNode Left { get; }
    public TokenType Operator { get; }
    public ASTNode Right { get; }

    public BinaryExpression(ASTNode left, TokenType op, ASTNode right)
    {
        Left = left;
        Operator = op;
        Right = right;
    }
}