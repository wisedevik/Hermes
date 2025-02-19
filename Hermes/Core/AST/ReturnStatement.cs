namespace Hermes.Core.AST;

internal class ReturnStatement : ASTNode
{
    public ASTNode Value { get; }

    public ReturnStatement(ASTNode value)
    {
        Value = value;
    }
}