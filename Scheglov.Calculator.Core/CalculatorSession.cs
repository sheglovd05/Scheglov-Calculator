using Scheglov.Calculator.Core.Models;
using Scheglov.Calculator.Core.Storage;

namespace Scheglov.Calculator.Core;

/// <summary>
/// Связывает вычисления, журнал и настройки в одно состояние, с которым работает
/// модель представления. Ядро при этом остаётся независимым от WPF.
/// </summary>
public sealed class CalculatorSession
{
    private readonly HistoryStore _history;
    private readonly SettingsStore _settings;

    public CalculatorSession(string? storageDirectory = null)
    {
        _history = new HistoryStore(storageDirectory);
        _settings = new SettingsStore(storageDirectory);
    }

    public IReadOnlyList<HistoryEntry> History => _history.Entries;

    public ApplicationSettings Settings => _settings.Current;

    /// <summary>
    /// Пробный расчёт для живого предпросмотра: журнал не пополняется.
    /// </summary>
    public CalculationResult Preview(string expression) => Calculator.TryEvaluate(expression);

    public CalculationResult Calculate(string expression)
    {
        var result = Calculator.TryEvaluate(expression);

        if (result.IsSuccess)
            _history.Add(new HistoryEntry(expression.Trim(), result.Value, DateTime.UtcNow));

        return result;
    }

    public void ClearHistory() => _history.Clear();

    public void UpdateSettings(ApplicationSettings settings) => _settings.Save(settings);

    public void ToggleTheme()
    {
        var current = Settings;
        var next = current with
        {
            Theme = current.Theme == ThemeMode.Dark ? ThemeMode.Light : ThemeMode.Dark
        };

        UpdateSettings(next);
    }

    public void ToggleLanguage()
    {
        var current = Settings;
        var next = current with
        {
            Language = current.Language == UiLanguage.Russian ? UiLanguage.English : UiLanguage.Russian
        };

        UpdateSettings(next);
    }
}
