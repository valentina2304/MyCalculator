using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Tema1Calculator
{
    public class CalculatorViewModel : INotifyPropertyChanged
    {
        private readonly CalculatorEngine _calculatorEngine;
        private string _displayText;
        private string _operationHistory;
        private bool _digitGroupingEnabled;


        public event PropertyChangedEventHandler PropertyChanged;

        public CalculatorViewModel()
        {
            _calculatorEngine = new CalculatorEngine();
            _displayText ="0";
            _operationHistory = "";
            _digitGroupingEnabled = false;
        }

        public string DisplayText
        {
            get => _displayText;
            set
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
                string decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                if (digit == "." || digit == ",")
                {
                    digit = decimalSeparator;
                }
                _calculatorEngine.ValidateAndEnterDigit(digit);
                UpdateDisplay(_calculatorEngine.CurrentValue);
            }
            catch (OverflowException)
            {
                MessageBox.Show("Overflow", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Invalid Input", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void SetOperation(string operation)
        {
            try
            {
                string result = _calculatorEngine.SetOperation(operation).ToString();
                UpdateDisplayWithHistory(result, operation);
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Invalid Operation", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void Calculate()
        {
            try
            {
                string result = _calculatorEngine.Calculate().ToString();
                UpdateDisplayWithHistory(result, "=");
            }
            catch (DivideByZeroException)
            {
                MessageBox.Show("Divide by Zero", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception)
            {
                MessageBox.Show("Error", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);    
            }
        }

        private void UpdateDisplay(double value)
        {
            DisplayText = FormatNumber(value.ToString(CultureInfo.InvariantCulture));
        }



        public bool DigitGroupingEnabled
        {
            get => _digitGroupingEnabled;
            set
            {
                _digitGroupingEnabled = value;
                OnPropertyChanged(nameof(DigitGroupingEnabled));
                UpdateDisplay(_calculatorEngine.CurrentValue);
            }
        }

        private void UpdateDisplayWithHistory(string value, string operation)
        {
            DisplayText = value;

            if (operation == "=")
            {
                OperationHistory = "";
            }
            else
            {
                OperationHistory += $"{FormatNumber(value)} {operation} ";
            }
        }

        private string FormatNumber(string valueStr)
        {
            if (double.TryParse(valueStr, System.Globalization.NumberStyles.Any,
                                System.Globalization.CultureInfo.InvariantCulture, out double value))
            {
                if (DigitGroupingEnabled)
                {
                    if (value == Math.Floor(value))
                    {
                        return value.ToString("N0", System.Globalization.CultureInfo.CurrentCulture);
                    }
                    else
                    {
                        int decimalPlaces = 0;
                        int decimalIndex = valueStr.IndexOf('.');

                        if (decimalIndex >= 0)
                        {
                            decimalPlaces = valueStr.Length - decimalIndex - 1;
                        }

                        return value.ToString($"N{decimalPlaces}", System.Globalization.CultureInfo.CurrentCulture);
                    }
                }
                else
                {
                    return valueStr;
                }
            }

            return valueStr;
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
               MessageBox.Show("Error", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                MessageBox.Show("Error", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                MessageBox.Show("Divide by Zero", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                MessageBox.Show("Error", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                MessageBox.Show("Invalid Input", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                MessageBox.Show("Error", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Funcții de memorie

        public List<double> GetMemoryList()
        {
            return _calculatorEngine.GetMemoryList();
        }

        public void UseValueFromMemory(double value)
        {
            _calculatorEngine.UseValueFromMemory(value);
            UpdateDisplay(_calculatorEngine.CurrentValue);
        }

        public void MemoryClear() => _calculatorEngine.ClearMemoryList();
        public void MemoryStore() => _calculatorEngine.MemoryStore();
        public void MemoryRecall()
        {
            double memoryValue = _calculatorEngine.RecallMemory();
            UpdateDisplay(memoryValue);
        }

        public void MemoryAdd() => _calculatorEngine.MemoryAdd();
        public void MemorySubtract() => _calculatorEngine.MemorySubtract();


        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
