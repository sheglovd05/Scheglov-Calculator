namespace Scheglov.Calculator.Core.Models;

public sealed record ApplicationSettings
{
    public ThemeMode Theme { get; init; } = ThemeMode.Dark;

    public UiLanguage Language { get; init; } = UiLanguage.Russian;

    public string AccentColor { get; init; } = "#4F8CFF";

    public static ApplicationSettings Default => new();
}
