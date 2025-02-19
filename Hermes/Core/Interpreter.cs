using Hermes.Core.AST;
using Hermes.Core.Config;

namespace Hermes.Core;

internal class Interpreter
{
    private Dictionary<string, FunctionDeclaration> functions = new();
    private Dictionary<string, object> variables = new();
    private HashSet<string> _importedModules = new HashSet<string>();

    private Dictionary<string, Dictionary<string, Func<object[], object>>> modules = new()
    {
        ["io"] = new()
        {
            ["print"] = args =>
            {
                var formattedArgs = args.Select(arg =>
                {
                    if (arg is List<object> list)
                    {
                        return "[" + string.Join(", ", list) + "]";
                    }
                    return arg?.ToString() ?? "null";
                });
                Console.WriteLine(string.Join(" ", formattedArgs));
                return null;
            },
            ["input"] = args => Console.ReadLine()
        },
        ["math"] = new()
        {
            ["sqrt"] = args => Math.Sqrt(Convert.ToDouble(args[0])),
            ["pow"] = args => Math.Pow(Convert.ToDouble(args[0]), Convert.ToDouble(args[1])),
            ["sin"] = args => Math.Sin(Convert.ToDouble(args[0])),
            ["cos"] = args => Math.Cos(Convert.ToDouble(args[0])),
            ["tan"] = args => Math.Tan(Convert.ToDouble(args[0])),
            ["log"] = args => Math.Log(Convert.ToDouble(args[0])),
            ["exp"] = args => Math.Exp(Convert.ToDouble(args[0])),
            ["abs"] = args => Math.Abs(Convert.ToDouble(args[0])),
            ["round"] = args => Math.Round(Convert.ToDouble(args[0])),
            ["floor"] = args => Math.Floor(Convert.ToDouble(args[0])),
            ["ceil"] = args => Math.Ceiling(Convert.ToDouble(args[0])),
        }
    };

    public void Interpret(ProgramNode program)
    {
        foreach (var stmt in program.Statements.OfType<FunctionDeclaration>())
        {
            functions[stmt.Name] = stmt;
        }

        foreach (var stmt in program.Statements)
        {
            if (stmt is FunctionDeclaration) continue;

            switch (stmt)
            {
                case VariableDeclaration varDecl:
                    variables[varDecl.Name] = EvaluateNode(varDecl.Value);
                    break;
                case ImportStatement import:
                    if (import.ModuleName == "io")
                    {
                        RegisterBuiltinModule("io", new Dictionary<string, Func<object[], object>>
                        {
                            ["print"] = args =>
                            {
                                var formattedArgs = args.Select(arg =>
                                {
                                    if (arg is List<object> list)
                                    {
                                        return "[" + string.Join(", ", list) + "]";
                                    }
                                    return arg?.ToString() ?? "null";
                                });
                                Console.WriteLine(string.Join(" ", formattedArgs));
                                return null;
                            },
                            ["input"] = args => Console.ReadLine()
                        });
                        _importedModules.Add("io");
                    }
                    else
                    {
                        LoadAndRegisterModule(import.ModuleName);
                    }
                    break;
                case FunctionCall call:
                    EvaluateFunctionCallAndReturn(call);
                    break;
                case ReturnStatement ret:
                    throw new Exception("Return outside of function");
                case ModuleNode moduleNode:
                    ExecuteModule(moduleNode);
                    break;
                default:
                    EvaluateNode(stmt);
                    break;
            }
        }
    }

    private void RegisterBuiltinModule(string name, Dictionary<string, Func<object[], object>> functions)
    {
        if (!modules.ContainsKey(name))
        {
            modules[name] = functions;
        }
    }

    private object ExecuteModule(ModuleNode moduleNode)
    {
        Console.WriteLine($"Loading the module: {moduleNode.ModuleName}");
        Interpret(moduleNode.Body);
        return null;
    }

    private object EvaluateProgramNode(ProgramNode prog)
    {
        object result = null;
        foreach (var stmt in prog.Statements)
        {
            result = EvaluateNode(stmt);
            if (stmt is ReturnStatement)
                break;
        }
        return result;
    }

    private object EvaluateNode(ASTNode node) => node switch
    {
        IfStatement ifStmt => EvaluateIfStatement(ifStmt),

        Literal lit => lit.Value switch
        {
            string numStr when double.TryParse(numStr, out var num) => num,
            string s => s,
            _ => lit.Value
        },

        Variable varNode => variables.TryGetValue(varNode.Name, out var value)
            ? value
            : throw new Exception($"Undefined variable: {varNode.Name}"),

        FunctionCall call => EvaluateFunctionCallAndReturn(call),

        BinaryExpression bin => EvaluateBinary(bin),

        ArrayAccess arrayAccess => EvaluateArrayAccess(arrayAccess),

        ReturnStatement ret => EvaluateNode(ret.Value),

        ProgramNode prog => EvaluateProgramNode(prog),

        _ => throw new Exception($"Unsupported node type: {node.GetType()}")
    };

    private object EvaluateArrayAccess(ArrayAccess arrayAccess)
    {
        var array = EvaluateNode(arrayAccess.Array);
        var index = EvaluateNode(arrayAccess.Index);

        if (array is List<object> list && index is double idx)
        {
            int i = (int)idx;
            if (i < 0 || i >= list.Count)
                throw new Exception($"Index {i} out of range for list of size {list.Count}");
            return list[i];
        }

        throw new Exception($"Cannot index type {array?.GetType()} with {index?.GetType()}");
    }

    private object EvaluateIfStatement(IfStatement ifStmt)
    {
        var condition = EvaluateNode(ifStmt.Condition);
        if (IsTruthy(condition))
        {
            return EvaluateProgramNode(ifStmt.Body);
        }

        foreach (var elseIf in ifStmt.ElseIfs)
        {
            var elseIfCondition = EvaluateNode(elseIf.Condition);
            if (IsTruthy(elseIfCondition))
            {
                return EvaluateProgramNode(elseIf.Body);
            }
        }

        if (ifStmt.Else != null)
        {
            return EvaluateProgramNode(ifStmt.Else.Body);
        }

        return null;
    }

    private bool IsTruthy(object value)
    {
        if (value is bool b)
            return b;
        if (value is double d)
            return d != 0;
        if (value is string s)
            return !string.IsNullOrEmpty(s);
        return value != null;
    }


    private object EvaluateBinary(BinaryExpression bin)
    {
        var left = EvaluateNode(bin.Left);
        var right = EvaluateNode(bin.Right);

        return bin.Operator switch
        {
            TokenType.Plus when left is string || right is string => $"{left}{right}",

            TokenType.Plus => Convert.ToDouble(left) + Convert.ToDouble(right),
            TokenType.Minus => Convert.ToDouble(left) - Convert.ToDouble(right),
            TokenType.Multiply => Convert.ToDouble(left) * Convert.ToDouble(right),
            TokenType.GreaterThan => Convert.ToDouble(left) > Convert.ToDouble(right),
            TokenType.LessThan => Convert.ToDouble(left) < Convert.ToDouble(right),
            TokenType.GreaterOrEqual => Convert.ToDouble(left) >= Convert.ToDouble(right),
            TokenType.LessOrEqual => Convert.ToDouble(left) <= Convert.ToDouble(right),
            TokenType.Equal => Equals(left, right),
            TokenType.NotEqual => !Equals(left, right),
            _ => throw new Exception($"Unknown operator: {bin.Operator}")
        };
    }


    private object EvaluateFunctionCallAndReturn(FunctionCall call)
    {
        if (functions.TryGetValue(call.FunctionName, out var func))
        {
            var literals = call.Arguments
                .Select(arg => arg is Literal literal ? literal : new Literal(EvaluateNode(arg).ToString()))
                .ToList();
            return ExecuteUserFunction(func, call.Arguments);
        }

        var parts = call.FunctionName.Split('.');
        if (parts.Length != 2)
        {
            throw new Exception($"Invalid function call format: {call.FunctionName}");
        }

        string moduleName = parts[0];
        string functionName = parts[1];

        if (!_importedModules.Contains(moduleName))
        {
            throw new Exception($"Module '{moduleName}' is not imported. Use '@import {moduleName}'.");
        }

        if (!modules.ContainsKey(moduleName))
        {
            throw new Exception($"Module '{moduleName}' is not loaded.");
        }

        if (modules[moduleName].TryGetValue(functionName, out var moduleFunc))
        {
            var args = call.Arguments.Select(EvaluateNode).ToArray();
            return moduleFunc(args);
        }

        throw new Exception($"Unknown function: {call.FunctionName}");
    }

    private void LoadAndRegisterModule(string moduleName)
    {
        var moduleLoader = new ModuleLoader(ConfigManager.Config.ModulePath);
        try
        {
            var module = moduleLoader.LoadModuleByName(moduleName);

            var moduleFunctions = new Dictionary<string, Func<object[], object>>();

            foreach (var stmt in module.Body.Statements.OfType<FunctionDeclaration>())
            {
                moduleFunctions[stmt.Name] = args =>
                {
                    var astNodes = args.Select(arg => (ASTNode)new Literal(arg.ToString())).ToList();
                    return ExecuteUserFunction(stmt, astNodes);
                };
            }

            modules[moduleName] = moduleFunctions;
            Interpret(module.Body);
        }
        catch (Exception e)
        {
            ; // лень!
        }

        _importedModules.Add(moduleName);
    }

    private void EvaluateFunctionCall(FunctionCall call)
    {
        var parts = call.FunctionName.Split('.');
        if (parts.Length != 2 || !modules.ContainsKey(parts[0]))
            throw new Exception($"Unknown function: {call.FunctionName}");

        var args = call.Arguments.Select(EvaluateNode).ToArray();
        modules[parts[0]][parts[1]](args);
    }

    private object ExecuteUserFunction(FunctionDeclaration func, List<ASTNode> args)
    {
        if (args.Count != func.Parameters.Count)
            throw new Exception($"Function {func.Name} expects {func.Parameters.Count} arguments, got {args.Count}");

        var prevVariables = variables;
        variables = new Dictionary<string, object>(prevVariables);

        for (int i = 0; i < func.Parameters.Count; i++)
        {
            variables[func.Parameters[i]] = EvaluateNode(args[i]);
        }

        object result = null;
        foreach (var stmt in func.Body)
        {
            result = EvaluateNode(stmt);
            if (stmt is ReturnStatement)
                break;
        }

        variables = prevVariables;
        return result;
    }
}