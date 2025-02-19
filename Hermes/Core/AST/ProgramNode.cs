namespace Hermes.Core.AST;

internal class ProgramNode : ASTNode
{
    public List<ASTNode> Statements { get; } = new List<ASTNode>();
}
