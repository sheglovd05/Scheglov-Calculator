using System.Globalization;

namespace Scheglov.Calculator.Core.Syntax;

/// <summary>
/// Превращает строку выражения в последовательность токенов.
/// Десятичным разделителем считается и точка, и запятая, поэтому ввод с клавиатуры
/// и кнопок панели даёт один и тот же результат.
/// </summary>
public static class Lexer
{
    public static IReadOnlyList<Token> Tokenize(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
            throw new CalculatorException("Выражение пустое.");

        var tokens = new List<Token>(expression.Length);
        var index = 0;

        while (index < expression.Length)
        {
            var symbol = expression[index];

            if (char.IsWhiteSpace(symbol))
            {
                index++;
                continue;
            }

            if (char.IsDigit(symbol) || IsSeparator(symbol))
            {
                index = ReadNumber(expression, index, tokens);
                continue;
            }

            var kind = symbol switch
            {
                '+' => TokenKind.Plus,
                '-' or '\u2212' => TokenKind.Minus,
                '*' or '\u00D7' or '\u00B7' => TokenKind.Multiply,
                '/' or '\u00F7' or ':' => TokenKind.Divide,
                '^' => TokenKind.Power,
                '(' => TokenKind.OpenParen,
                ')' => TokenKind.CloseParen,
                _ => throw new CalculatorException($"Недопустимый символ «{symbol}».")
            };

            tokens.Add(Token.ForOperator(kind, index));
            index++;
        }

        if (tokens.Count == 0)
            throw new CalculatorException("Выражение пустое.");

        tokens.Add(Token.ForOperator(TokenKind.End, expression.Length));
        return tokens;
    }

    private static int ReadNumber(string expression, int start, List<Token> tokens)
    {
        var index = start;
        var separatorSeen = false;

        while (index < expression.Length)
        {
            var symbol = expression[index];

            if (char.IsDigit(symbol))
            {
                index++;
                continue;
            }

            if (IsSeparator(symbol))
            {
                if (separatorSeen)
                    break;

                separatorSeen = true;
                index++;
                continue;
            }

            break;
        }

        var text = expression[start..index].Replace(',', '.');

        if (!decimal.TryParse(text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var value))
            throw new CalculatorException($"Не удалось разобрать число «{text}».");

        tokens.Add(Token.ForNumber(value, start));
        return index;
    }

    private static bool IsSeparator(char symbol) => symbol is '.' or ',';
}
