using System.Windows;
using System.Windows.Input;
using Scheglov.Calculator.App.ViewModels;

namespace Scheglov.Calculator.App;

public partial class MainWindow : Window
{
    private const string AllowedSymbols = "0123456789.,+-*/^()";

    public MainWindow()
    {
        InitializeComponent();
    }

    private MainViewModel ViewModel => (MainViewModel)DataContext;

    /// <summary>
    /// Печать символов идёт через TextInput, чтобы одинаково работать
    /// с основной клавиатурой, цифровым блоком и раскладкой.
    /// </summary>
    private void OnWindowTextInput(object sender, TextCompositionEventArgs e)
    {
        foreach (var symbol in e.Text)
        {
            if (AllowedSymbols.Contains(symbol))
            {
                ViewModel.Append(symbol.ToString());
                e.Handled = true;
                continue;
            }

            var normalized = symbol switch
            {
                '×' => '*',
                '÷' => '/',
                '−' => '-',
                _ => '\0'
            };

            if (normalized == '\0')
                continue;

            ViewModel.Append(normalized.ToString());
            e.Handled = true;
        }
    }

    private void OnWindowKeyDown(object sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Enter:
                ViewModel.Evaluate();
                e.Handled = true;
                break;

            case Key.Back:
                ViewModel.Backspace();
                e.Handled = true;
                break;

            case Key.Escape:
                ViewModel.ClearAll();
                e.Handled = true;
                break;

            case Key.Delete:
                ViewModel.ClearEntry();
                e.Handled = true;
                break;
        }
    }
}
