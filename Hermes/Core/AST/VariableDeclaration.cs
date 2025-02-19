namespace Hermes.Core.AST;

internal class VariableDeclaration : ASTNode
{
    public string Name { get; }
    public ASTNode Value { get; }

    public VariableDeclaration(string name, ASTNode value)
    {
        Name = name;
        Value = value;
    }
}