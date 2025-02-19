namespace Hermes.Core.AST;

internal class ModuleNode : ASTNode
{
    public string ModuleName { get; }
    public ProgramNode Body { get; }

    public ModuleNode(string moduleName, ProgramNode body)
    {
        ModuleName = moduleName;
        Body = body;
    }
}