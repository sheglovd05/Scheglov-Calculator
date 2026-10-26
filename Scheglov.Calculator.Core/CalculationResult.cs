namespace Scheglov.Calculator.Core;

/// <summary>
/// Результат вычисления. Ошибка возвращается как значение, а не как исключение,
/// потому что предпросмотр результата пересчитывается при каждом нажатии клавиши
/// и незавершённое выражение — это нормальная ситуация, а не сбой.
/// </summary>
public readonly record struct CalculationResult(bool IsSuccess, decimal Value, string? Error)
{
    public static CalculationResult Success(decimal value) => new(true, value, null);

    public static CalculationResult Failure(string error) => new(false, 0m, error);
}
