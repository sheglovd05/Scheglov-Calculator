using Microsoft.VisualStudio.TestTools.UnitTesting;
using Scheglov.Calculator.Core;
using Scheglov.Calculator.Core.Models;
using Scheglov.Calculator.Core.Storage;

namespace Scheglov.Calculator.Core.Tests;

[TestClass]
public class CalculatorSessionTests
{
    private string _storage = null!;

    [TestInitialize]
    public void CreateStorage()
    {
        _storage = Path.Combine(Path.GetTempPath(), "calc-tests-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_storage);
    }

    [TestCleanup]
    public void RemoveStorage()
    {
        if (Directory.Exists(_storage))
            Directory.Delete(_storage, recursive: true);
    }

    [TestMethod]
    public void Calculate_AddsEntryToHistory()
    {
        var session = new CalculatorSession(_storage);

        session.Calculate("2+2");

        Assert.AreEqual(1, session.History.Count);
        Assert.AreEqual("2+2", session.History[0].Expression);
        Assert.AreEqual(4m, session.History[0].Result);
    }

    [TestMethod]
    public void Preview_DoesNotTouchHistory()
    {
        var session = new CalculatorSession(_storage);

        session.Preview("2+2");

        Assert.AreEqual(0, session.History.Count);
    }

    [TestMethod]
    public void Calculate_FailedExpression_IsNotStored()
    {
        var session = new CalculatorSession(_storage);

        var result = session.Calculate("2+");

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual(0, session.History.Count);
    }

    [TestMethod]
    public void History_IsLoadedAfterRestart()
    {
        var first = new CalculatorSession(_storage);
        first.Calculate("3*3");

        var second = new CalculatorSession(_storage);

        Assert.AreEqual(1, second.History.Count);
        Assert.AreEqual(9m, second.History[0].Result);
    }

    [TestMethod]
    public void History_KeepsOnlyLastEntriesWhenCapacityExceeded()
    {
        var store = new HistoryStore(_storage);

        for (var index = 0; index < HistoryStore.Capacity + 5; index++)
            store.Add(new HistoryEntry($"1+{index}", 1 + index, DateTime.UtcNow));

        Assert.AreEqual(HistoryStore.Capacity, store.Entries.Count);
        Assert.AreEqual("1+5", store.Entries[0].Expression);
    }

    [TestMethod]
    public void ClearHistory_LeavesEmptyJournal()
    {
        var session = new CalculatorSession(_storage);
        session.Calculate("1+1");

        session.ClearHistory();

        Assert.AreEqual(0, session.History.Count);
        Assert.AreEqual(0, new CalculatorSession(_storage).History.Count);
    }

    [TestMethod]
    public void ToggleTheme_SwitchesAndPersists()
    {
        var session = new CalculatorSession(_storage);
        var expected = session.Settings.Theme == ThemeMode.Dark ? ThemeMode.Light : ThemeMode.Dark;

        session.ToggleTheme();

        Assert.AreEqual(expected, session.Settings.Theme);
        Assert.AreEqual(expected, new CalculatorSession(_storage).Settings.Theme);
    }

    [TestMethod]
    public void ToggleLanguage_IsPersisted()
    {
        var session = new CalculatorSession(_storage);
        session.ToggleLanguage();

        var language = session.Settings.Language;

        Assert.AreEqual(language, new CalculatorSession(_storage).Settings.Language);
    }

    [TestMethod]
    public void SettingsStore_WithBrokenFile_FallsBackToDefaults()
    {
        File.WriteAllText(Path.Combine(_storage, "settings.json"), "{ это не json");

        var store = new SettingsStore(_storage);

        Assert.AreEqual(ApplicationSettings.Default.Theme, store.Current.Theme);
    }
}
