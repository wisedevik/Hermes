namespace Hermes.Core.AST;

internal class ArrayAccess : ASTNode
{
    public ASTNode Array { get; }
    public ASTNode Index { get; }

    public ArrayAccess(ASTNode array, ASTNode index)
    {
        Array = array;
        Index = index;
    }

    public override string ToString()
    {
        return $"{Array}[{Index}]";
    }
}