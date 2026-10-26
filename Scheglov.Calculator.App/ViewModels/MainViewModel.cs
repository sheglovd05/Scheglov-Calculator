using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;
using Scheglov.Calculator.App.Infrastructure;
using Scheglov.Calculator.App.Presentation;
using Scheglov.Calculator.Core;

namespace Scheglov.Calculator.App.ViewModels;

public sealed class MainViewModel : INotifyPropertyChanged
{
    private readonly CalculatorSession _session;
    private readonly DelegateCommand _evaluateCommand;

    private ThemePalette _palette;
    private UiStrings _texts;

    private string _expression = string.Empty;
    private string _previewText = string.Empty;

    public MainViewModel()
    {
        _session = new CalculatorSession();
        _palette = ThemePalette.Create(_session.Settings.Theme, _session.Settings.AccentColor);
        _texts = Localization.For(_session.Settings.Language);

        HistoryRows = new ObservableCollection<HistoryRow>();

        AppendCommand = new DelegateCommand(parameter => Append(parameter as string ?? string.Empty));
        ClearCommand = new DelegateCommand(_ => ClearAll());
        ClearEntryCommand = new DelegateCommand(_ => ClearEntry());
        BackspaceCommand = new DelegateCommand(_ => Backspace());
        ClearHistoryCommand = new DelegateCommand(_ => ClearHistory());
        ToggleThemeCommand = new DelegateCommand(_ => ToggleTheme());
        ToggleLanguageCommand = new DelegateCommand(_ => ToggleLanguage());
        UseHistoryCommand = new DelegateCommand(parameter => UseHistory(parameter as HistoryRow));

        _evaluateCommand = new DelegateCommand(_ => Evaluate(), _ => !string.IsNullOrWhiteSpace(Expression));
        EvaluateCommand = _evaluateCommand;

        RefreshHistory();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<HistoryRow> HistoryRows { get; }

    public ICommand AppendCommand { get; }
    public ICommand ClearCommand { get; }
    public ICommand ClearEntryCommand { get; }
    public ICommand BackspaceCommand { get; }
    public ICommand ClearHistoryCommand { get; }
    public ICommand ToggleThemeCommand { get; }
    public ICommand ToggleLanguageCommand { get; }
    public ICommand UseHistoryCommand { get; }
    public ICommand EvaluateCommand { get; }

    public string Expression
    {
        get => _expression;
        set
        {
            if (!SetProperty(ref _expression, value ?? string.Empty))
                return;

            _evaluateCommand.RaiseCanExecuteChanged();
            UpdatePreview();
        }
    }

    public string PreviewText
    {
        get => _previewText;
        private set => SetProperty(ref _previewText, value);
    }

    public Brush PreviewBrush
    {
        get => _previewBrush;
        private set => SetProperty(ref _previewBrush, value);
    }

    private Brush _previewBrush = Brushes.Gray;

    public string WindowTitle => _texts.WindowTitle;
    public string AuthorText => _texts.Author;
    public string HistoryTitleText => _texts.HistoryTitle;
    public string ClearHistoryText => _texts.ClearHistory;
    public string ExpressionLabelText => _texts.ExpressionLabel;
    public string ResultLabelText => _texts.ResultLabel;
    public string ThemeButtonText => _texts.ThemeButton;
    public string LanguageButtonText => _texts.LanguageButton;
    public string HintText => _texts.Hint;
    public string EmptyHistoryText => _texts.EmptyHistory;

    public Brush PageBrush => _palette.Page;
    public Brush SurfaceBrush => _palette.Surface;
    public Brush KeyBrush => _palette.Key;
    public Brush AccentBrush => _palette.Accent;
    public Brush OnAccentBrush => _palette.OnAccent;
    public Brush TextBrush => _palette.Text;
    public Brush MutedBrush => _palette.Muted;
    public Brush BorderBrush => _palette.Border;
    public Brush ErrorBrush => _palette.Error;

    public void Append(string token)
    {
        if (string.IsNullOrEmpty(token))
            return;

        Expression += token;
    }

    public void Backspace()
    {
        if (Expression.Length == 0)
            return;

        Expression = Expression[..^1];
    }

    public void ClearAll()
    {
        Expression = string.Empty;
        PreviewText = string.Empty;
    }

    /// <summary>
    /// Убирает последнее введённое число, оставляя операцию и скобки.
    /// </summary>
    public void ClearEntry()
    {
        if (string.IsNullOrWhiteSpace(Expression))
            return;

        var end = Expression.Length;

        while (end > 0 && (char.IsDigit(Expression[end - 1]) || Expression[end - 1] is '.' or ','))
            end--;

        Expression = Expression[..end];
        PreviewText = string.Empty;
    }

    public void Evaluate()
    {
        if (string.IsNullOrWhiteSpace(Expression))
            return;

        var result = _session.Calculate(Expression);

        if (result.IsSuccess)
        {
            PreviewText = result.Value.ToString(CultureInfo.InvariantCulture);
            PreviewBrush = _palette.Text;
            RefreshHistory();
            return;
        }

        PreviewText = _texts.ErrorPrefix + result.Error;
        PreviewBrush = _palette.Error;
    }

    public void ClearHistory()
    {
        _session.ClearHistory();
        HistoryRows.Clear();
    }

    public void ToggleTheme()
    {
        _session.ToggleTheme();
        _palette = ThemePalette.Create(_session.Settings.Theme, _session.Settings.AccentColor);

        OnPropertyChanged(nameof(PageBrush));
        OnPropertyChanged(nameof(SurfaceBrush));
        OnPropertyChanged(nameof(KeyBrush));
        OnPropertyChanged(nameof(AccentBrush));
        OnPropertyChanged(nameof(OnAccentBrush));
        OnPropertyChanged(nameof(TextBrush));
        OnPropertyChanged(nameof(MutedBrush));
        OnPropertyChanged(nameof(BorderBrush));
        OnPropertyChanged(nameof(ErrorBrush));

        UpdatePreview();
    }

    public void ToggleLanguage()
    {
        _session.ToggleLanguage();
        _texts = Localization.For(_session.Settings.Language);

        OnPropertyChanged(nameof(WindowTitle));
        OnPropertyChanged(nameof(AuthorText));
        OnPropertyChanged(nameof(HistoryTitleText));
        OnPropertyChanged(nameof(ClearHistoryText));
        OnPropertyChanged(nameof(ExpressionLabelText));
        OnPropertyChanged(nameof(ResultLabelText));
        OnPropertyChanged(nameof(ThemeButtonText));
        OnPropertyChanged(nameof(LanguageButtonText));
        OnPropertyChanged(nameof(HintText));
        OnPropertyChanged(nameof(EmptyHistoryText));
    }

    private void UseHistory(HistoryRow? row)
    {
        if (row is null)
            return;

        Expression = row.Expression;
        Evaluate();
    }

    private void UpdatePreview()
    {
        if (string.IsNullOrWhiteSpace(Expression))
        {
            PreviewText = string.Empty;
            return;
        }

        var result = _session.Preview(Expression);

        if (result.IsSuccess)
        {
            PreviewText = result.Value.ToString(CultureInfo.InvariantCulture);
            PreviewBrush = _palette.Muted;
            return;
        }

        PreviewText = string.Empty;
    }

    private void RefreshHistory()
    {
        HistoryRows.Clear();

        var entries = _session.History;
        for (var index = entries.Count - 1; index >= 0; index--)
            HistoryRows.Add(new HistoryRow(entries[index]));
    }

    private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private void OnPropertyChanged(string? propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
