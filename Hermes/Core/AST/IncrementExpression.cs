namespace Hermes.Core.AST;

internal class IncrementExpression : ASTNode
{
    public Variable Target { get; }
    public bool IsPrefix { get; }

    public IncrementExpression(Variable target, bool isPrefix)
    {
        Target = target;
        IsPrefix = isPrefix;
    }
}