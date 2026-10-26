namespace Scheglov.Calculator.Core.Syntax;

public enum BinaryOperator
{
    Add,
    Subtract,
    Multiply,
    Divide,
    Power
}

public abstract record ExpressionNode;

public sealed record NumberNode(decimal Value) : ExpressionNode;

public sealed record NegateNode(ExpressionNode Operand) : ExpressionNode;

public sealed record BinaryNode(BinaryOperator Operator, ExpressionNode Left, ExpressionNode Right) : ExpressionNode;
