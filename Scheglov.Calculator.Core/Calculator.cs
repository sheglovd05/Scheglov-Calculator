using Scheglov.Calculator.Core.Syntax;

namespace Scheglov.Calculator.Core;

/// <summary>
/// Чистая математика без истории и файлов: разбор строки и вычисление.
/// </summary>
public static class Calculator
{
    public static decimal Evaluate(string expression)
    {
        var tree = Parser.Parse(expression);
        return Evaluator.Evaluate(tree);
    }

    public static CalculationResult TryEvaluate(string expression)
    {
        try
        {
            return CalculationResult.Success(Evaluate(expression));
        }
        catch (CalculatorException error)
        {
            return CalculationResult.Failure(error.Message);
        }
        catch (OverflowException)
        {
            return CalculationResult.Failure("Результат выходит за допустимый диапазон чисел.");
        }
    }
}
