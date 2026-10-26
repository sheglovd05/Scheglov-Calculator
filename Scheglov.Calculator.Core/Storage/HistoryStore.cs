using Scheglov.Calculator.Core.Models;

namespace Scheglov.Calculator.Core.Storage;

/// <summary>
/// Журнал вычислений. Хранит не больше заданного количества записей,
/// чтобы файл не разрастался бесконечно.
/// </summary>
public sealed class HistoryStore
{
    public const int Capacity = 100;

    private readonly string _path;
    private readonly List<HistoryEntry> _entries;

    public HistoryStore(string? storageDirectory = null)
    {
        _path = Path.Combine(storageDirectory ?? AppStorage.DefaultDirectory, "history.json");
        _entries = JsonFileStore.Load(_path, () => new List<HistoryEntry>());
    }

    public IReadOnlyList<HistoryEntry> Entries => _entries;

    public void Add(HistoryEntry entry)
    {
        _entries.Add(entry);

        if (_entries.Count > Capacity)
            _entries.RemoveRange(0, _entries.Count - Capacity);

        Save();
    }

    public void Clear()
    {
        _entries.Clear();
        Save();
    }

    public void Save() => JsonFileStore.Save(_path, _entries);
}
