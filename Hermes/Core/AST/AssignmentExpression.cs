namespace Hermes.Core.AST;

internal class AssignmentExpression : ASTNode
{
    public ASTNode Left { get; }
    public ASTNode Value { get; }

    public AssignmentExpression(ASTNode left, ASTNode value)
    {
        Left = left;
        Value = value;
    }
}
