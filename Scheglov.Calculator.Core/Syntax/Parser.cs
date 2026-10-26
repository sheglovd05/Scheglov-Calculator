namespace Scheglov.Calculator.Core.Syntax;

/// <summary>
/// Рекурсивный спуск по грамматике выражений.
/// Приоритеты операций заданы самой структурой методов, поэтому отдельная
/// таблица приоритетов и перевод в постфиксную запись не нужны.
/// </summary>
public sealed class Parser
{
    private readonly IReadOnlyList<Token> _tokens;
    private int _position;

    private Parser(IReadOnlyList<Token> tokens)
    {
        _tokens = tokens;
    }

    public static ExpressionNode Parse(string expression)
    {
        var parser = new Parser(Lexer.Tokenize(expression));
        var root = parser.ParseAdditive();

        if (parser.Current.Kind != TokenKind.End)
            throw new CalculatorException(
                $"Неожидаемый фрагмент «{parser.Current.Describe()}» в позиции {parser.Current.Position + 1}.");

        return root;
    }

    private Token Current => _tokens[_position];

    private Token Advance()
    {
        var token = _tokens[_position];

        if (_position < _tokens.Count - 1)
            _position++;

        return token;
    }

    private ExpressionNode ParseAdditive()
    {
        var left = ParseMultiplicative();

        while (Current.Kind is TokenKind.Plus or TokenKind.Minus)
        {
            var operation = Advance().Kind == TokenKind.Plus
                ? BinaryOperator.Add
                : BinaryOperator.Subtract;

            left = new BinaryNode(operation, left, ParseMultiplicative());
        }

        return left;
    }

    private ExpressionNode ParseMultiplicative()
    {
        var left = ParseUnary();

        while (Current.Kind is TokenKind.Multiply or TokenKind.Divide)
        {
            var operation = Advance().Kind == TokenKind.Multiply
                ? BinaryOperator.Multiply
                : BinaryOperator.Divide;

            left = new BinaryNode(operation, left, ParseUnary());
        }

        return left;
    }

    private ExpressionNode ParseUnary()
    {
        if (Current.Kind == TokenKind.Minus)
        {
            Advance();
            return new NegateNode(ParseUnary());
        }

        return ParsePower();
    }

    private ExpressionNode ParsePower()
    {
        var left = ParsePrimary();

        if (Current.Kind != TokenKind.Power)
            return left;

        Advance();

        // Правая часть разбирается как унарное выражение, поэтому степень
        // правоассоциативна и допускает отрицательный показатель: 2 ^ -3.
        return new BinaryNode(BinaryOperator.Power, left, ParseUnary());
    }

    private ExpressionNode ParsePrimary()
    {
        if (Current.Kind == TokenKind.Number)
            return new NumberNode(Advance().Number);

        if (Current.Kind == TokenKind.OpenParen)
        {
            var opening = Advance().Position;
            var inner = ParseAdditive();

            if (Current.Kind != TokenKind.CloseParen)
                throw new CalculatorException($"Не закрыта скобка, открытая в позиции {opening + 1}.");

            Advance();
            return inner;
        }

        if (Current.Kind == TokenKind.End)
            throw new CalculatorException("Выражение закончилось раньше, чем ожидалось.");

        throw new CalculatorException(
            $"Ожидалось число или скобка, получено «{Current.Describe()}» в позиции {Current.Position + 1}.");
    }
}
