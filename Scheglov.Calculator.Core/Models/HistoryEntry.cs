namespace Scheglov.Calculator.Core.Models;

public sealed record HistoryEntry(string Expression, decimal Result, DateTime CreatedAtUtc)
{
    public DateTime CreatedAtLocal => CreatedAtUtc.ToLocalTime();
}
