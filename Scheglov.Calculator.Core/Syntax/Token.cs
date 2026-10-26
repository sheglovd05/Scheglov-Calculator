namespace Scheglov.Calculator.Core.Syntax;

public enum TokenKind
{
    Number,
    Plus,
    Minus,
    Multiply,
    Divide,
    Power,
    OpenParen,
    CloseParen,
    End
}

public readonly record struct Token(TokenKind Kind, decimal Number, int Position)
{
    public static Token ForOperator(TokenKind kind, int position) => new(kind, 0m, position);

    public static Token ForNumber(decimal value, int position) => new(TokenKind.Number, value, position);

    public string Describe() => Kind switch
    {
        TokenKind.Number => Number.ToString(System.Globalization.CultureInfo.InvariantCulture),
        TokenKind.Plus => "+",
        TokenKind.Minus => "-",
        TokenKind.Multiply => "*",
        TokenKind.Divide => "/",
        TokenKind.Power => "^",
        TokenKind.OpenParen => "(",
        TokenKind.CloseParen => ")",
        TokenKind.End => "конец выражения",
        _ => Kind.ToString()
    };
}
