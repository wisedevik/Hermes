using Hermes.Core.AST;

namespace Hermes.Core;

internal class ModuleLoader
{
    private readonly string modulesPath;

    public ModuleLoader(string modulesPath)
    {
        this.modulesPath = modulesPath;
    }

    public ModuleNode LoadModuleByName(string moduleName)
    {
        Dictionary<string, string> moduleMap = ScanModules();
        if (!moduleMap.ContainsKey(moduleName))
            throw new Exception($"Module {moduleName} not found");

        string filePath = moduleMap[moduleName];
        string code = File.ReadAllText(filePath);
        Lexer lexer = new Lexer(code);
        Parser parser = new Parser(lexer);
        return parser.ParseModule();
    }

    public string ExtractModuleName(string filePath)
    {
        string code = File.ReadAllText(filePath);
        Lexer lexer = new Lexer(code);
        Parser parser = new Parser(lexer);
        ModuleNode moduleNode = parser.ParseModule();
        return moduleNode.ModuleName;
    }

    public Dictionary<string, string> ScanModules()
    {
        string[] moduleFiles = Directory.GetFiles(modulesPath, "*.hs");
        Dictionary<string, string> moduleMap = new Dictionary<string, string>();

        foreach (string filePath in moduleFiles)
        {
            try
            {
                string moduleName = ExtractModuleName(filePath);
                moduleMap[moduleName] = filePath;
            }
            catch
            {
            }
        }

        return moduleMap;
    }
}
