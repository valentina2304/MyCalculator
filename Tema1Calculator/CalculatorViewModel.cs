using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Tema1Calculator
{
    public class CalculatorViewModel : INotifyPropertyChanged
    {
        private readonly CalculatorEngine _calculatorEngine;
        private string _displayText;
        private string _operationHistory;

        public event PropertyChangedEventHandler PropertyChanged;

        public CalculatorViewModel()
        {
            _calculatorEngine = new CalculatorEngine();
            _displayText = "0";
            _operationHistory = "";
        }

        public string DisplayText
        {
            get => _displayText;
            private set
            {
                _displayText = value;
                OnPropertyChanged();
            }
        }

        public string OperationHistory
        {
            get => _operationHistory;
            private set
            {
                _operationHistory = value;
                OnPropertyChanged();
            }
        }


        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (char.IsDigit((char)e.Key) || e.Key == Key.Decimal || e.Key == Key.OemPeriod)
            {
                string digit = e.Key == Key.Decimal || e.Key == Key.OemPeriod ? "." :
                    ((int)e.Key - (int)Key.D0).ToString();
                EnterDigit(digit);
                e.Handled = true;
            }
            else if (e.Key == Key.Add || e.Key == Key.Subtract ||
                     e.Key == Key.Multiply || e.Key == Key.Divide)
            {
                string operation = GetOperationFromKey(e.Key);
                SetOperation(operation);
                e.Handled = true;
            }
            else if (e.Key == Key.Enter)
            {
                Calculate();
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                Clear();
                e.Handled = true;
            }
        }

        private string GetOperationFromKey(Key key)
        {
            return key switch
            {
                Key.Add => "+",
                Key.Subtract => "-",
                Key.Multiply => "*",
                Key.Divide => "/",
                _ => ""
            };
        }

        public void EnterDigit(string digit)
        {
            try
            {
                _calculatorEngine.ValidateAndEnterDigit(digit);
                UpdateDisplay(_calculatorEngine.CurrentValue);
            }
            catch (OverflowException)
            {
                DisplayText = "Overflow";
            }
            catch (ArgumentException)
            {
                DisplayText = "Invalid Input";
            }
        }

        public void SetOperation(string operation)
        {
            try
            {
                double result = _calculatorEngine.SetOperation(operation);
                UpdateDisplayWithHistory(result, operation);
            }
            catch (ArgumentException)
            {
                DisplayText = "Invalid Operation";
            }
        }

        public void Calculate()
        {
            try
            {
                double result = _calculatorEngine.Calculate();
                UpdateDisplayWithHistory(result, "=");
            }
            catch (DivideByZeroException)
            {
                DisplayText = "Divide by Zero";
            }
            catch (Exception)
            {
                DisplayText = "Error";
            }
        }

        private void UpdateDisplay(double value)
        {
            DisplayText = FormatNumber(value);
        }

        private bool _digitGroupingEnabled = false;

        public bool DigitGroupingEnabled
        {
            get => _digitGroupingEnabled;
            set
            {
                _digitGroupingEnabled = value;
                OnPropertyChanged();
                // Re-format the current display with the new setting
                UpdateDisplay(_calculatorEngine.CurrentValue);
            }
        }

        private void UpdateDisplayWithHistory(double value, string operation)
        {
            DisplayText = FormatNumber(value);

            if (operation == "=")
            {
                OperationHistory = "";
            }
            else
            {
                OperationHistory += $"{FormatNumber(value)} {operation} ";
            }
        }

        private string FormatNumber(double value)
        {
            if (DigitGroupingEnabled)
            {
                // Use current culture to properly format the number with appropriate separators
                return value.ToString("N", System.Globalization.CultureInfo.CurrentCulture);
            }
            else
            {
                // Original formatting without digit grouping
                return value.ToString("0.##########");
            }
        }

        public void Clear()
        {
            _calculatorEngine.Reset();
            DisplayText = "0";
            OperationHistory = "";
        }

        // Funcții pentru operații speciale

        public void Backspace()
        {
            try
            {
                UpdateDisplay(_calculatorEngine.Backspace());
            }
            catch (Exception)
            {
                DisplayText = "Error";
            }
        }

        public void ClearEntry()
        {
            _calculatorEngine.ClearEntry();
            UpdateDisplay(_calculatorEngine.CurrentValue);
        }

        public void ChangeSign()
        {
            try
            {
                UpdateDisplay(_calculatorEngine.ChangeSign());
            }
            catch (Exception)
            {
                DisplayText = "Error";
            }
        }

        public void Reciprocal()
        {
            try
            {
                UpdateDisplay(_calculatorEngine.Reciprocal());
            }
            catch (DivideByZeroException)
            {
                DisplayText = "Divide by Zero";
            }
        }

        public void Square()
        {
            try
            {
                UpdateDisplay(_calculatorEngine.Square());
            }
            catch (Exception)
            {
                DisplayText = "Error";
            }
        }

        public void SquareRoot()
        {
            try
            {
                UpdateDisplay(_calculatorEngine.SquareRoot());
            }
            catch (ArithmeticException)
            {
                DisplayText = "Invalid Input";
            }
        }

        public void Percentage()
        {
            try
            {
                UpdateDisplay(_calculatorEngine.Percentage());
            }
            catch (Exception)
            {
                DisplayText = "Error";
            }
        }

        // Funcții de memorie
        public void MemoryClear() => _calculatorEngine.MemoryClear();
        public void MemoryStore() => _calculatorEngine.MemoryStore();
        public void MemoryRecall()
        {
            _calculatorEngine.MemoryRecall();
            UpdateDisplay(_calculatorEngine.CurrentValue);
        }
        public void MemoryAdd() => _calculatorEngine.MemoryAdd();
        public void MemorySubtract() => _calculatorEngine.MemorySubtract();


        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
