using System.Text;

namespace Hermes.Core;

internal class Lexer
{
    private readonly string _input;
    private int _position;

    public Lexer(string input)
    {
        this._input = input;
    }

    public Token NextToken()
    {
        while (true)
        {
            if (_position >= _input.Length)
                return new Token(TokenType.EOF);

            char current = _input[_position];


            // skip whitespaces
            if (char.IsWhiteSpace(current))
            {
                _position++;
                continue;
            }
            

            if (current == 'i' && LookAhead(2) == "if")
            {
                _position += 2;
                return new Token(TokenType.IfKeyword);
            }

            if (current == 'e' && LookAhead(4) == "else")
            {
                _position += 4;
                return new Token(TokenType.ElseKeyword);
            }

            if (current == '!' && LookAhead(7) == "!module")
            {
                _position += 7;
                while (_position < _input.Length && char.IsWhiteSpace(_input[_position]))
                    _position++;
                if (_position >= _input.Length || _input[_position] != '[')
                    throw new Exception("Expected '[' after the !module");
                _position++;
                while (_position < _input.Length && char.IsWhiteSpace(_input[_position]))
                    _position++;
                if (_position >= _input.Length || _input[_position] != '"')
                    throw new Exception("Expected '\"' for the module name");
                _position++;
                int start = _position;
                while (_position < _input.Length && _input[_position] != '"')
                    _position++;
                if (_position >= _input.Length)
                    throw new Exception("The closing character was not found '\"' for the module name");
                string moduleName = _input.Substring(start, _position - start);
                _position++;
                while (_position < _input.Length && char.IsWhiteSpace(_input[_position]))
                    _position++;
                if (_position >= _input.Length || _input[_position] != ']')
                    throw new Exception("Expected ']' after the module name");
                _position++;
                return new Token(TokenType.ModuleDeclaration, moduleName);
            }

            if (current == 'f' && LookAhead(4) == "func")
            {
                _position += 4;
                return new Token(TokenType.FuncKeyword);

            }

            if (current == 'r' && LookAhead(6) == "return")
            {
                _position += 6;
                return new Token(TokenType.ReturnKeyword);
            }


            if (current == 'v' && LookAhead(3) == "var")
            {
                _position += 3;
                return new Token(TokenType.VarKeyword);
            }

            // Directive @import processing
            if (current == '@')
            {
                _position++;
                var keyword = ReadWhile(c => char.IsLetter(c));

                if (keyword == "import")
                {
                    ReadWhile(c => char.IsWhiteSpace(c));
                    return new Token(TokenType.ImportKeyword);
                }

                throw new Exception($"Unknown directive: @{keyword}");
            }

            // identifiers

            if (char.IsLetter(current) || current == '_')
            {
                var ident = ReadWhile(c => char.IsLetterOrDigit(c) || c == '_' || c == '.');
                return new Token(TokenType.Identifier, ident);
            }

            // strings
            if (current == '"')
            {
                _position++;
                var str = ReadWhile(c => c != '"');
                _position++;
                return new Token(TokenType.String, str);
            }

            // numbers
            if (char.IsDigit(current) || current == '.')
            {
                var num = ReadWhile(c => char.IsDigit(c) || c == '.');
                return new Token(TokenType.Number, num);
            }

            if (current == '[')
            {
                _position++;
                return new Token(TokenType.LeftBracket);
            }
            if (current == ']')
            {
                _position++;
                return new Token(TokenType.RightBracket);
            }

            // symbols
            switch (current)
            {
                case '(':
                    _position++;
                    return new Token(TokenType.LeftParen);
                case ')':
                    _position++;
                    return new Token(TokenType.RightParen);
                case ',':
                    _position++;
                    return new Token(TokenType.Comma);
                case ';':
                    _position++;
                    return new Token(TokenType.Semicolon);
                case '+':
                    _position++;
                    return new Token(TokenType.Plus);
                case '-':
                    _position++;
                    return new Token(TokenType.Minus);
                case '*':
                    _position++;
                    return new Token(TokenType.Multiply);
                case '/':
                    _position++;
                    return new Token(TokenType.Divide);
                case '%':
                    _position++;
                    return new Token(TokenType.Modulo);
                case '{':
                    _position++;
                    return new Token(TokenType.LeftBrace);
                case '}':
                    _position++;
                    return new Token(TokenType.RightBrace);
                case '>':
                    if (LookAhead(2) == ">=")
                    {
                        _position += 2;
                        return new Token(TokenType.GreaterOrEqual);
                    }
                    else
                    {
                        _position++;
                        return new Token(TokenType.GreaterThan);
                    }
                case '<':
                    if (LookAhead(2) == "<=")
                    {
                        _position += 2;
                        return new Token(TokenType.LessOrEqual);
                    }
                    else
                    {
                        _position++;
                        return new Token(TokenType.LessThan);
                    }
                case '=':
                    if (LookAhead(2) == "==")
                    {
                        _position += 2;
                        return new Token(TokenType.Equal);
                    }
                    else
                    {
                        _position++;
                        return new Token(TokenType.Assign);
                    }
                case '!':
                    if (LookAhead(2) == "!=")
                    {
                        _position += 2;
                        return new Token(TokenType.NotEqual);
                    }
                    else
                    {
                        throw new Exception($"Unexpected character: {current}");
                    }
            }

            throw new Exception($"Unexpected character: {current}");
        }
    }

    private string LookAhead(int length)
    {
        var result = new StringBuilder();
        for (int i = 0; i < length; i++)
        {
            int pos = _position + i;
            if (pos >= _input.Length) break;
            result.Append(_input[pos]);
        }
        return result.ToString();
    }

    private string ReadWhile(Func<char, bool> condition)
    {
        int start = _position;
        while (_position < _input.Length && condition(_input[_position]))
        {
            _position++;
        }
        return _input.Substring(start, _position - start);
    }
}
