using Scheglov.Calculator.Core.Models;

namespace Scheglov.Calculator.Core.Storage;

public sealed class SettingsStore
{
    private readonly string _path;
    private ApplicationSettings _current;

    public SettingsStore(string? storageDirectory = null)
    {
        _path = Path.Combine(storageDirectory ?? AppStorage.DefaultDirectory, "settings.json");
        _current = JsonFileStore.Load(_path, () => ApplicationSettings.Default);
    }

    public ApplicationSettings Current => _current;

    public void Save(ApplicationSettings settings)
    {
        _current = settings;
        JsonFileStore.Save(_path, settings);
    }
}
