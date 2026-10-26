using System.Windows.Media;
using Scheglov.Calculator.Core.Models;

namespace Scheglov.Calculator.App.Presentation;

/// <summary>
/// Набор кистей для одной темы. Модель представления отдаёт их окну напрямую,
/// поэтому конвертеры цветов в XAML не нужны.
/// </summary>
public sealed record ThemePalette(
    Brush Page,
    Brush Surface,
    Brush Key,
    Brush Accent,
    Brush OnAccent,
    Brush Text,
    Brush Muted,
    Brush Border,
    Brush Error)
{
    public static ThemePalette Create(ThemeMode theme, string accentColor)
    {
        var accent = ParseColor(accentColor, Color.FromRgb(0x4F, 0x8C, 0xFF));

        if (theme == ThemeMode.Light)
        {
            return new ThemePalette(
                Page: Frozen("#F2F5FA"),
                Surface: Frozen("#FFFFFF"),
                Key: Frozen("#E8EEF8"),
                Accent: Frozen(accent),
                OnAccent: Frozen("#FFFFFF"),
                Text: Frozen("#141821"),
                Muted: Frozen("#5E6878"),
                Border: Frozen("#D5DEEC"),
                Error: Frozen("#C0392B"));
        }

        return new ThemePalette(
            Page: Frozen("#0E1219"),
            Surface: Frozen("#161C26"),
            Key: Frozen("#212936"),
            Accent: Frozen(accent),
            OnAccent: Frozen("#FFFFFF"),
            Text: Frozen("#EDF1F9"),
            Muted: Frozen("#8B96AA"),
            Border: Frozen("#2A3341"),
            Error: Frozen("#FF6B5E"));
    }

    private static Brush Frozen(string hex) => Frozen(ParseColor(hex, Colors.Gray));

    private static Brush Frozen(Color color)
    {
        var brush = new SolidColorBrush(color);
        brush.Freeze();
        return brush;
    }

    private static Color ParseColor(string hex, Color fallback)
    {
        if (string.IsNullOrWhiteSpace(hex))
            return fallback;

        try
        {
            return (Color)ColorConverter.ConvertFromString(hex)!;
        }
        catch (FormatException)
        {
            return fallback;
        }
    }
}
