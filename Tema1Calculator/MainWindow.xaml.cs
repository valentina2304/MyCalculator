using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Tema1Calculator;

namespace Tema1Calculator
{
    public partial class MainWindow : Window
    {
        private readonly CalculatorViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new CalculatorViewModel();
            DataContext = _viewModel;
            AttachButtonHandlers();
        }

        // In MainWindow.xaml.cs, modify AttachButtonHandlers to include these buttons
        private void AttachButtonHandlers()
        {
            foreach (UIElement element in (FindName("Grid") as Grid).Children)
            {
                if (element is Button button)
                {
                    string content = button.Content.ToString();
                    button.Click += (s, e) => HandleButtonClick(content);
                }
            }

            // Add handlers for memory buttons in the StackPanel
            foreach (UIElement element in (FindName("MemoryButtonsPanel") as StackPanel).Children)
            {
                if (element is Button button)
                {
                    string content = button.Content.ToString();
                    button.Click += (s, e) => HandleButtonClick(content);
                }
            }
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Created by Lixandru Valentina-Mariana 10LF332", "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void DigitGrouping_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.DigitGroupingEnabled = true;
        }
        private void HandleButtonClick(string content)
        {
            switch (content)
            {
                case "0":
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                case ".":
                    _viewModel.EnterDigit(content);
                    break;
                case "+":
                case "-":
                case "*":
                case "/":
                    _viewModel.SetOperation(content);
                    break;
                case "=":
                    _viewModel.Calculate();
                    break;
                case "C":
                    _viewModel.Clear();
                    break;
                case "⌫":
                    _viewModel.Backspace();
                    break;
                case "CE":
                    _viewModel.ClearEntry();
                    break;
                case "±":
                    _viewModel.ChangeSign();
                    break;
                case "⅟x":
                    _viewModel.Reciprocal();
                    break;
                case "x²":
                    _viewModel.Square();
                    break;
                case "√x":
                    _viewModel.SquareRoot();
                    break;
                case "%":
                    _viewModel.Percentage();
                    break;
                case "M+":
                    _viewModel.MemoryAdd();
                    break;
                case "M-":
                    _viewModel.MemorySubtract();
                    break;
                case "MR":
                    _viewModel.MemoryRecall();
                    break;
                case "MC":
                    _viewModel.MemoryClear();
                    break;
                case "M":
                    _viewModel.MemoryStore();
                    break;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // For number keys (D0-D9)
            if (e.Key >= Key.D0 && e.Key <= Key.D9)
            {
                string digit = (e.Key - Key.D0).ToString();
                _viewModel.EnterDigit(digit);
                e.Handled = true;
                return;
            }

            // For number pad keys (NumPad0-NumPad9)
            if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                string digit = (e.Key - Key.NumPad0).ToString();
                _viewModel.EnterDigit(digit);
                e.Handled = true;
                return;
            }

            // For other keys
            switch (e.Key)
            {
                case Key.Decimal:
                case Key.OemPeriod:
                    _viewModel.EnterDigit(".");
                    break;
                case Key.Add:
                    _viewModel.SetOperation("+");
                    break;
                case Key.Subtract:
                    _viewModel.SetOperation("-");
                    break;
                case Key.Multiply:
                    _viewModel.SetOperation("*");
                    break;
                case Key.Divide:
                    _viewModel.SetOperation("/");
                    break;
                case Key.Enter:
                    _viewModel.Calculate();
                    break;
                case Key.Escape:
                    _viewModel.Clear();
                    break;
            }

            e.Handled = true;
        }
    


    private void CutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            string text = _viewModel.DisplayText;
            Clipboard.SetText(text);
            _viewModel.ClearEntry();
        }

        private void CopyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            string text = _viewModel.DisplayText;
            Clipboard.SetText(text);
        }

        private void PasteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                string text = Clipboard.GetText();

                // Clear current entry before pasting
                _viewModel.ClearEntry();

                bool hasDecimal = false;
                foreach (char c in text)
                {
                    if (char.IsDigit(c))
                    {
                        _viewModel.EnterDigit(c.ToString());
                    }
                    else if (c == '.' && !hasDecimal)
                    {
                        _viewModel.EnterDigit(".");
                        hasDecimal = true;
                    }
                    // Ignore other characters
                }
            }
        }

    }
}
    