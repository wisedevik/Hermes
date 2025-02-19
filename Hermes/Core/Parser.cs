using Hermes.Core.AST;

namespace Hermes.Core;

internal class Parser
{
    private readonly Lexer _lexer;
    private Token _currentToken;

    public Parser(Lexer lexer)
    {
        this._lexer = lexer;
        this._currentToken = lexer.NextToken();
    }

    public ModuleNode ParseModule()
    {
        if (_currentToken.Type != TokenType.ModuleDeclaration)
            throw new Exception("The module file should start with !module[...]");

        string moduleName = _currentToken.Value;
        Eat(TokenType.ModuleDeclaration);
        var body = ParseProgram();
        return new ModuleNode(moduleName, body);
    }

    public ProgramNode ParseProgram()
    {
        var program = new ProgramNode();

        while (_currentToken.Type != TokenType.EOF && _currentToken.Type != TokenType.RightBrace)
        {
            if(_currentToken.Type == TokenType.IfKeyword)
            {
                program.Statements.Add(ParseIfStatement());
            }
            else if (_currentToken.Type == TokenType.ImportKeyword)
            {
                program.Statements.Add(ParseImport());
            }
            else if (_currentToken.Type == TokenType.VarKeyword)
            {
                program.Statements.Add(ParseMultipleAssignment());
            }
            else if (_currentToken.Type == TokenType.FuncKeyword)
            {
                program.Statements.Add(ParseFunctionDeclaration());
            }
            else if (_currentToken.Type == TokenType.ReturnKeyword)
            {
                program.Statements.Add(ParseReturnStatement());
            }
            else if (_currentToken.Type == TokenType.ForKeyword)
            {
                program.Statements.Add(ParseForStatement());
            }
            else
            {
                program.Statements.Add(ParseExpression());
            }

            if (_currentToken.Type == TokenType.Semicolon)
            {
                Eat(TokenType.Semicolon);
            }
        }

        return program;
    }

    private ASTNode ParseMultipleAssignment()
    {
        Eat(TokenType.VarKeyword);
        var variables = new List<string>();
        do
        {
            if (_currentToken.Type != TokenType.Identifier)
                throw new Exception("Expected variable name");

            variables.Add(_currentToken.Value);
            Eat(TokenType.Identifier);

            if (_currentToken.Type != TokenType.Comma)
                break;

            Eat(TokenType.Comma);
        } while (true);

        Eat(TokenType.Assign);

        var values = new List<ASTNode>();
        do
        {
            values.Add(ParseExpression());

            if (_currentToken.Type != TokenType.Comma)
                break;

            Eat(TokenType.Comma);
        } while (true);

        if (variables.Count != values.Count)
            throw new Exception("Number of variables and values must match");

        return new MultipleAssignment(variables, values);
    }

    private ForStatement ParseForStatement()
    {
        Eat(TokenType.ForKeyword);
        Eat(TokenType.LeftParen);

        ASTNode initializer = null;
        if (_currentToken.Type != TokenType.Semicolon)
            initializer = _currentToken.Type == TokenType.VarKeyword ?
                ParseVariableDeclaration()
                : ParseExpression();

        Eat(TokenType.Semicolon);
        ASTNode condition = new Literal("1");
        if (_currentToken.Type != TokenType.Semicolon)
        {
            condition = ParseExpression();
        }

        Eat(TokenType.Semicolon);

        ASTNode increment = null;
        if (_currentToken.Type != TokenType.RightParen)
        {
            increment = ParseExpression();
        }
        Eat(TokenType.RightParen);

        Eat(TokenType.LeftBrace);
        var body = ParseProgram();
        Eat(TokenType.RightBrace);

        return new ForStatement(initializer, condition, increment, body);
    }

    private ASTNode ParseArrayAccess(ASTNode array)
    {
        Eat(TokenType.LeftBracket);
        var index = ParseExpression();
        Eat(TokenType.RightBracket);
        return new ArrayAccess(array, index);
    }

    private ArrayLiteral ParseArrayLiteral()
    {
        Eat(TokenType.LeftBracket);

        var elements = new List<ASTNode>();

        while (_currentToken.Type != TokenType.RightBracket)
        {
            elements.Add(ParseExpression());

            if (_currentToken.Type == TokenType.Comma)
            {
                Eat(TokenType.Comma);
            }
        }

        Eat(TokenType.RightBracket);

        return new ArrayLiteral(elements);
    }

    private IfStatement ParseIfStatement()
    {
        Eat(TokenType.IfKeyword);

        ASTNode condition = ParseExpression();

        // body of if
        Eat(TokenType.LeftBrace);
        var body = ParseProgram();
        Eat(TokenType.RightBrace);

        // else of else if
        List<ElseIfStatement> elseIfs = new();
        ElseStatement elseStatement = null;

        while (_currentToken.Type == TokenType.ElseKeyword)
        {
            Eat(TokenType.ElseKeyword);

            if (_currentToken.Type == TokenType.IfKeyword)
            {
                // else if
                Eat(TokenType.IfKeyword);
                var elseIfCondition = ParseExpression();
                Eat(TokenType.LeftBrace);
                var elseIfBody = ParseProgram();
                Eat(TokenType.RightBrace);
                elseIfs.Add(new ElseIfStatement(elseIfCondition, elseIfBody));
            }
            else
            {
                // else
                Eat(TokenType.LeftBrace);
                var elseBody = ParseProgram();
                Eat(TokenType.RightBrace);
                elseStatement = new ElseStatement(elseBody);
                break;
            }
        }

        return new IfStatement(condition, body, elseIfs, elseStatement);
    }

    private FunctionDeclaration ParseFunctionDeclaration()
    {
        Eat(TokenType.FuncKeyword);
        var name = _currentToken.Value;
        Eat(TokenType.Identifier);

        Eat(TokenType.LeftParen);
        var parameters = new List<string>();
        while (_currentToken.Type != TokenType.RightParen)
        {
            parameters.Add(_currentToken.Value);
            Eat(TokenType.Identifier);
            if (_currentToken.Type == TokenType.Comma) Eat(TokenType.Comma);
        }
        Eat(TokenType.RightParen);

        List<ASTNode> body;

        if (_currentToken.Type == TokenType.Arrow)
        {
            Eat(TokenType.Arrow);
            var expression = ParseExpression();
            body = new List<ASTNode> { new ReturnStatement(expression) };
            Eat(TokenType.Semicolon); 
        }
        else
        {
            Eat(TokenType.LeftBrace);
            body = new List<ASTNode>();
            while (_currentToken.Type != TokenType.RightBrace)
            {
                body.Add(ParseProgram());
                if (_currentToken.Type == TokenType.Semicolon) Eat(TokenType.Semicolon);
            }
            Eat(TokenType.RightBrace);
        }

        return new FunctionDeclaration(name, parameters, body);
    }

    private ASTNode ParseReturnStatement()
    {
        Eat(TokenType.ReturnKeyword);
        var value = ParseExpression();
        return new ReturnStatement(value);
    }

    private VariableDeclaration ParseVariableDeclaration()
    {
        Eat(TokenType.VarKeyword);
        var name = _currentToken.Value;
        Eat(TokenType.Identifier);
        Eat(TokenType.Assign);
        var value = ParseExpression();
        return new VariableDeclaration(name, value);
    }

    private ImportStatement ParseImport()
    {
        Eat(TokenType.ImportKeyword);
        var moduleToken = _currentToken;
        Eat(TokenType.Identifier);
        return new ImportStatement(moduleToken.Value);
    }

    private ASTNode ParseExpression()
    {
        return ParseAssignment();
    }

    private ASTNode ParseAssignment()
    {
        var left = ParseComparison();

        if (_currentToken.Type == TokenType.Assign)
        {
            Eat(TokenType.Assign);
            var value = ParseAssignment();
            return new AssignmentExpression(left, value);
        }

        return left;
    }

    private ASTNode ParseComparison()
    {
        var left = ParseAdditive();

        while (_currentToken.Type is TokenType.GreaterThan or TokenType.LessThan
               or TokenType.GreaterOrEqual or TokenType.LessOrEqual
               or TokenType.Equal or TokenType.NotEqual)
        {
            var op = _currentToken.Type;
            Eat(op);
            var right = ParseAdditive();
            left = new BinaryExpression(left, op, right);
        }

        return left;
    }

    private ASTNode ParseAdditive()
    {
        var left = ParseMultiplicative();

        while (_currentToken.Type == TokenType.Plus || _currentToken.Type == TokenType.Minus)
        {
            var op = _currentToken.Type;
            Eat(op);
            var right = ParseMultiplicative();
            left = new BinaryExpression(left, op, right);
        }

        return left;
    }

    private ASTNode ParseMultiplicative()
    {
        var left = ParsePrimary();

        while (_currentToken.Type == TokenType.Multiply || _currentToken.Type == TokenType.Divide || _currentToken.Type == TokenType.Modulo)
        {
            var op = _currentToken.Type;
            Eat(op);
            var right = ParsePrimary();
            left = new BinaryExpression(left, op, right);
        }

        return left;
    }

    private ASTNode ParseFunctionCall(string identifier)
    {
        Eat(TokenType.LeftParen);
        var args = new List<ASTNode>();

        while (_currentToken.Type != TokenType.RightParen)
        {
            args.Add(ParseExpression());
            if (_currentToken.Type == TokenType.Comma)
                Eat(TokenType.Comma);
        }

        Eat(TokenType.RightParen);
        return new FunctionCall(identifier, args);
    }

    private ASTNode ParsePrimary()
    {
        if (_currentToken.Type == TokenType.String ||
            _currentToken.Type == TokenType.Number)
        {
            var lit = new Literal(_currentToken.Value);
            Eat(_currentToken.Type);
            return lit;
        }

        if(_currentToken.Type is TokenType.Increment or TokenType.Decrement)
        {
            bool isIncrement = _currentToken.Type == TokenType.Increment;
            Eat(_currentToken.Type);

            var target = ParsePrimary() as Variable ??
                throw new Exception("Invalid target for increment/decrement");

            return isIncrement
                ? new IncrementExpression(target, true)
                : new DecrementExpression(target, true);
        }

        if (_currentToken.Type == TokenType.LeftParen)
        {
            Eat(TokenType.LeftParen);
            var expr = ParseExpression();
            Eat(TokenType.RightParen);
            return expr;
        }

        if (_currentToken.Type == TokenType.Identifier)
        {
            var ident = _currentToken.Value;
            Eat(TokenType.Identifier);

            if (_currentToken.Type is TokenType.Increment or TokenType.Decrement)
            {
                bool isIncrement = _currentToken.Type == TokenType.Increment;
                Eat(_currentToken.Type);

                return isIncrement
                    ? new IncrementExpression(new Variable(ident), false)
                    : new DecrementExpression(new Variable(ident), false);
            }
            else if (_currentToken.Type == TokenType.LeftParen)
            {
                return ParseFunctionCall(ident);
            }
            else if (_currentToken.Type == TokenType.LeftBracket)
            {
                var arrayNode = new Variable(ident);
                return ParseArrayAccess(arrayNode);
            }

            return new Variable(ident);
        }

        if (_currentToken.Type == TokenType.Assign)
        {
            throw new Exception("Unexpected assignment operator '=' in expression");
        }

        throw new Exception($"Unexpected token: {_currentToken.Type}");
    }

    private void Eat(TokenType type)
    {
        if (_currentToken.Type == type)
            _currentToken = _lexer.NextToken();
        else
            throw new Exception($"Expected {type}, got {_currentToken.Type}");
    }
}
