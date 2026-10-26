using Scheglov.Calculator.Core.Models;

namespace Scheglov.Calculator.App.Presentation;

public sealed record UiStrings(
    string WindowTitle,
    string Author,
    string HistoryTitle,
    string ClearHistory,
    string ExpressionLabel,
    string ResultLabel,
    string ThemeButton,
    string LanguageButton,
    string Hint,
    string EmptyHistory,
    string ErrorPrefix);

public static class Localization
{
    private static readonly UiStrings Russian = new(
        WindowTitle: "Калькулятор",
        Author: "Щеглов Даниил · ИПо-31",
        HistoryTitle: "Журнал вычислений",
        ClearHistory: "Очистить",
        ExpressionLabel: "Выражение",
        ResultLabel: "Результат",
        ThemeButton: "Тема",
        LanguageButton: "EN",
        Hint: "Поддерживаются + − × ÷ ^ и скобки",
        EmptyHistory: "Здесь появятся выполненные вычисления",
        ErrorPrefix: "Ошибка: ");

    private static readonly UiStrings English = new(
        WindowTitle: "Calculator",
        Author: "Daniil Scheglov · IPO-31",
        HistoryTitle: "History",
        ClearHistory: "Clear",
        ExpressionLabel: "Expression",
        ResultLabel: "Result",
        ThemeButton: "Theme",
        LanguageButton: "RU",
        Hint: "Supports + − × ÷ ^ and parentheses",
        EmptyHistory: "Completed calculations will appear here",
        ErrorPrefix: "Error: ");

    public static UiStrings For(UiLanguage language) =>
        language == UiLanguage.English ? English : Russian;
}
