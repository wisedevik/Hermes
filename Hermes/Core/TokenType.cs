namespace Hermes.Core;

internal enum TokenType
{
    ImportKeyword, Identifier, String, Number,
    LeftParen, RightParen, Comma, Semicolon, 
    VarKeyword, Assign, InputKeyword, Plus,
    Minus, Multiply, Divide, Modulo,
    FuncKeyword, LeftBrace, RightBrace, 
    ReturnKeyword, ModuleDeclaration, IfKeyword,
    ElseKeyword, ElseIfKeyword, GreaterThan, LessThan,
    GreaterOrEqual, LessOrEqual, Equal,
    NotEqual, LeftBracket, RightBracket, EOF
}
