using Hermes.Core;
using Hermes.Core.Config;
using Newtonsoft.Json;
using System.Text;

namespace Hermes;

internal class Program
{
    static Config? config = ConfigManager.Config;

    public static void Main(string[] args)
    {
        Console.Title = "BwLang | Interpreter";

        if (config == null)
        {
            Config genConfig = ConfigManager.GenerateConfig();
        }

        if (args.Length == 0)
        {
            RunFile("main.hs");
            return;
        }

        switch (args[0])
        {
            case "-i":
                RunREPL();
                break;

            case "-f":
                if (args.Length < 2)
                {
                    Console.WriteLine("Error: specify the path to the file after -f");
                    return;
                }
                RunFile(args[1]);
                break;

            default:
                Console.WriteLine("Usage: Hermes.exe [-i | -f <file>]");
                break;
        }
    }

    private static void RunFile(string filePath)
    {
        try
        {
            string code = File.ReadAllText(filePath);
            RunCode(code, new Interpreter());
        }
        catch (Exception e)
        {
            Console.WriteLine($"File reading error {filePath}: {e.Message}\n{e.StackTrace}");
        }
    }

    private static void RunREPL()
    {
        Console.WriteLine($"Hermes ({config.Version}) REPL - Enter the code line by line (exit to exit)");

        var interpreter = new Interpreter();

        while (true)
        {
            Console.Write(">>> ");
            string? line = Console.ReadLine();

            if (line == null || line.Trim() == "exit")
                break;

            try
            {
                RunCode(line, interpreter);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }
    }

    private static void RunCode(string code, Interpreter interpreter)
    {
        var lexer = new Lexer(code);
        var parser = new Parser(lexer);
        var program = parser.ParseProgram();
        interpreter.Interpret(program);
    }
}
