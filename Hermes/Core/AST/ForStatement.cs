namespace Hermes.Core.AST;

internal class ForStatement : ASTNode
{
    public ASTNode Initializer { get; }
    public ASTNode Condition { get; }
    public ASTNode Increment { get; }
    public ProgramNode Body { get; }

    public ForStatement(ASTNode initializer, ASTNode condition, ASTNode increment, ProgramNode body)
    {
        Initializer = initializer;
        Condition = condition;
        Increment = increment;
        Body = body;
    }
}
