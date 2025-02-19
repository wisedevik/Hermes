namespace Hermes.Core.AST;

internal class FunctionDeclaration : ASTNode
{
    public string Name { get; }
    public List<string> Parameters { get; }
    public List<ASTNode> Body { get; }

    public FunctionDeclaration(string name, List<string> parameters, List<ASTNode> body)
    {
        Name = name;
        Parameters = parameters;
        Body = body;
    }
}
