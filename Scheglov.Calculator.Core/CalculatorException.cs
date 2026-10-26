namespace Scheglov.Calculator.Core;

/// <summary>
/// Ошибка разбора или вычисления выражения. Сообщение показывается пользователю как есть.
/// </summary>
public sealed class CalculatorException : Exception
{
    public CalculatorException(string message) : base(message)
    {
    }
}
