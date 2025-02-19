namespace Hermes.Core.AST;

internal class IfStatement : ASTNode
{
    public ASTNode Condition { get; }
    public ProgramNode Body { get; }
    public List<ElseIfStatement> ElseIfs { get; }
    public ElseStatement Else { get; }

    public IfStatement(ASTNode condition, ProgramNode body, List<ElseIfStatement> elseIfs, ElseStatement elseStatement)
    {
        Condition = condition;
        Body = body;
        ElseIfs = elseIfs;
        Else = elseStatement;
    }
}
