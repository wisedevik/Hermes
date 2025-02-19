namespace Hermes.Core.AST;

internal class DecrementExpression : ASTNode
{
    public Variable Target { get; }
    public bool IsPrefix { get; }

    public DecrementExpression(Variable target, bool isPrefix)
    {
        Target = target;
        IsPrefix = isPrefix;
    }
}