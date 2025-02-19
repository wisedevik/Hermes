namespace Hermes.Core.AST;

internal class MultipleAssignment : ASTNode
{
    public List<string> Variables { get; }
    public List<ASTNode> Values { get; }

    public MultipleAssignment(List<string> variables, List<ASTNode> values)
    {
        Variables = variables;
        Values = values;
    }
}