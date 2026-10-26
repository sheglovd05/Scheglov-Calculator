using System.Globalization;
using Scheglov.Calculator.Core.Models;

namespace Scheglov.Calculator.App.ViewModels;

/// <summary>
/// Одна строка журнала в том виде, в котором она показывается в списке.
/// </summary>
public sealed class HistoryRow
{
    public HistoryRow(HistoryEntry entry)
    {
        Expression = entry.Expression;
        Result = entry.Result.ToString(CultureInfo.InvariantCulture);
        Time = entry.CreatedAtLocal.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
    }

    public string Expression { get; }

    public string Result { get; }

    public string Time { get; }
}
