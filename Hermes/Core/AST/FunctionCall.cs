namespace Hermes.Core.AST;

internal class FunctionCall : ASTNode
{
    public string FunctionName { get; }
    public List<ASTNode> Arguments { get; }

    public FunctionCall(string name, List<ASTNode> args)
    {
        FunctionName = name;
        Arguments = args;
    }
}
