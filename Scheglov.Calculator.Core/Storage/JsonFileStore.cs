using System.Text.Json;

namespace Scheglov.Calculator.Core.Storage;

/// <summary>
/// Небольшая обёртка над JSON-файлом: повреждённый или отсутствующий файл
/// не должен ронять приложение, поэтому в этом случае возвращаются значения по умолчанию.
/// </summary>
internal static class JsonFileStore
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true
    };

    public static T Load<T>(string path, Func<T> fallback)
    {
        try
        {
            if (!File.Exists(path))
                return fallback();

            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<T>(json, Options) ?? fallback();
        }
        catch (Exception error) when (error is IOException or JsonException or UnauthorizedAccessException)
        {
            return fallback();
        }
    }

    public static void Save<T>(string path, T value)
    {
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        File.WriteAllText(path, JsonSerializer.Serialize(value, Options));
    }
}
