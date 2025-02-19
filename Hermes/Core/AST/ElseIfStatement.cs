namespace Hermes.Core.AST;

internal class ElseIfStatement : ASTNode
{
    public ASTNode Condition { get; }
    public ProgramNode Body { get; }

    public ElseIfStatement(ASTNode condition, ProgramNode body)
    {
        Condition = condition;
        Body = body;
    }
}
