namespace Hermes.Core.AST;

internal class ImportStatement : ASTNode
{
    public string ModuleName { get; }

    public ImportStatement(string moduleName)
    {
        ModuleName = moduleName;
    }
}
