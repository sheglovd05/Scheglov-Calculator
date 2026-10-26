namespace Scheglov.Calculator.Core.Storage;

public static class AppStorage
{
    /// <summary>
    /// Каталог с данными приложения: %LOCALAPPDATA%\Scheglov\Calculator.
    /// </summary>
    public static string DefaultDirectory => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "Scheglov",
        "Calculator");
}
