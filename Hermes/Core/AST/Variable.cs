namespace Hermes.Core.AST;

internal class Variable : ASTNode
{
    public string Name { get; }

    public Variable(string name)
    {
        Name = name;
    }
}
