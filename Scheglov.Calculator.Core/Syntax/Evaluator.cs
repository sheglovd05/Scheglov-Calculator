namespace Scheglov.Calculator.Core.Syntax;

/// <summary>
/// Вычисляет значение дерева выражения. Денежные величины здесь ни при чём,
/// но decimal выбран сознательно: он не накапливает двоичную погрешность,
/// поэтому 0.1 + 0.2 даёт ровно 0.3, а не 0.30000000000000004.
/// </summary>
public static class Evaluator
{
    public static decimal Evaluate(ExpressionNode node) => node switch
    {
        NumberNode number => number.Value,
        NegateNode negate => checked(-Evaluate(negate.Operand)),
        BinaryNode binary => EvaluateBinary(binary),
        _ => throw new CalculatorException("Неподдерживаемая конструкция выражения.")
    };

    private static decimal EvaluateBinary(BinaryNode node)
    {
        var left = Evaluate(node.Left);
        var right = Evaluate(node.Right);

        try
        {
            return node.Operator switch
            {
                BinaryOperator.Add => checked(left + right),
                BinaryOperator.Subtract => checked(left - right),
                BinaryOperator.Multiply => checked(left * right),
                BinaryOperator.Divide => right == 0m
                    ? throw new CalculatorException("Деление на ноль невозможно.")
                    : left / right,
                BinaryOperator.Power => Power(left, right),
                _ => throw new CalculatorException("Неизвестная операция.")
            };
        }
        catch (OverflowException)
        {
            throw new CalculatorException("Результат выходит за допустимый диапазон чисел.");
        }
    }

    private static decimal Power(decimal value, decimal exponent)
    {
        if (exponent != decimal.Truncate(exponent))
            throw new CalculatorException("Показатель степени должен быть целым числом.");

        if (exponent < int.MinValue || exponent > int.MaxValue)
            throw new CalculatorException("Показатель степени слишком велик.");

        return Pow(value, (int)exponent);
    }

    private static decimal Pow(decimal value, int exponent)
    {
        if (exponent == 0)
            return 1m;

        if (exponent < 0)
        {
            if (value == 0m)
                throw new CalculatorException("Ноль нельзя возвести в отрицательную степень.");

            return 1m / PowPositive(value, -(long)exponent);
        }

        return PowPositive(value, exponent);
    }

    private static decimal PowPositive(decimal value, long exponent)
    {
        var result = 1m;
        var factor = value;

        while (exponent > 0)
        {
            if ((exponent & 1L) == 1L)
                result = checked(result * factor);

            exponent >>= 1;

            if (exponent > 0)
                factor = checked(factor * factor);
        }

        return result;
    }
}
