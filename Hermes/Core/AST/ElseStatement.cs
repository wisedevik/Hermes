namespace Hermes.Core.AST;

internal class ElseStatement : ASTNode
{
    public ProgramNode Body { get; }

    public ElseStatement(ProgramNode body)
    {
        Body = body;
    }
}