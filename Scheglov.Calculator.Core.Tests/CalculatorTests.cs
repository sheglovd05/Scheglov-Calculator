using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scheglov.Calculator.Core;

namespace Scheglov.Calculator.Core.Tests;

[TestClass]
public class CalculatorTests
{
    [DataTestMethod]
    [DataRow("2+3*4", 14)]
    [DataRow("(2+3)*4", 20)]
    [DataRow("10/4", 2.5)]
    [DataRow("2^10", 1024)]
    [DataRow("2^-2", 0.25)]
    [DataRow("-2^2", -4)]
    [DataRow("-(3+4)*2", -14)]
    [DataRow("7-2-1", 4)]
    [DataRow("100/10/2", 5)]
    public void Evaluate_RespectsOperatorPrecedence(string expression, double expected)
    {
        var result = Calculator.Evaluate(expression);

        Assert.AreEqual((decimal)expected, result);
    }

    [TestMethod]
    public void Evaluate_WithDecimalCommaAndPoint_GivesSameValue()
    {
        Assert.AreEqual(4m, Calculator.Evaluate("1,5+2,5"));
        Assert.AreEqual(4m, Calculator.Evaluate("1.5+2.5"));
    }

    [TestMethod]
    public void Evaluate_DecimalArithmetic_HasNoBinaryNoise()
    {
        Assert.AreEqual(0.3m, Calculator.Evaluate("0.1+0.2"));
    }

    [TestMethod]
    public void Evaluate_IgnoresSpaces()
    {
        Assert.AreEqual(12m, Calculator.Evaluate(" 3 * ( 1 + 3 ) "));
    }

    [TestMethod]
    public void Evaluate_DivisionByZero_Throws()
    {
        var error = Assert.ThrowsException<CalculatorException>(() => Calculator.Evaluate("5/0"));

        StringAssert.Contains(error.Message, "ноль");
    }

    [TestMethod]
    public void Evaluate_UnclosedParenthesis_Throws()
    {
        var error = Assert.ThrowsException<CalculatorException>(() => Calculator.Evaluate("(2+3"));

        StringAssert.Contains(error.Message, "скобка");
    }

    [TestMethod]
    public void Evaluate_EmptyExpression_Throws()
    {
        Assert.ThrowsException<CalculatorException>(() => Calculator.Evaluate("   "));
    }

    [TestMethod]
    public void Evaluate_UnknownSymbol_Throws()
    {
        var error = Assert.ThrowsException<CalculatorException>(() => Calculator.Evaluate("2%3"));

        StringAssert.Contains(error.Message, "%");
    }

    [TestMethod]
    public void Evaluate_TwoNumbersWithoutOperator_Throws()
    {
        Assert.ThrowsException<CalculatorException>(() => Calculator.Evaluate("2 3"));
    }

    [TestMethod]
    public void Evaluate_FractionalExponent_Throws()
    {
        var error = Assert.ThrowsException<CalculatorException>(() => Calculator.Evaluate("9^0.5"));

        StringAssert.Contains(error.Message, "целым");
    }

    [TestMethod]
    public void TryEvaluate_OnError_ReturnsFailureWithMessage()
    {
        var result = Calculator.TryEvaluate("1+");

        Assert.IsFalse(result.IsSuccess);
        Assert.IsNotNull(result.Error);
        Assert.AreEqual(0m, result.Value);
    }

    [TestMethod]
    public void TryEvaluate_OnSuccess_ReturnsValue()
    {
        var result = Calculator.TryEvaluate("6*7");

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(42m, result.Value);
        Assert.IsNull(result.Error);
    }
}
