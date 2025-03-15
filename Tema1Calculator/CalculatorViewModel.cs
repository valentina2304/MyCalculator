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
            _displayText = "0";
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

        public bool DigitGroupingEnabled
        {
            get => _digitGroupingEnabled;
            set
            {
                _digitGroupingEnabled = value;
                OnPropertyChanged();
                UpdateDisplay(_calculatorEngine.CurrentValue);
            }
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
            DisplayText = _calculatorEngine.FormatNumber(value, _digitGroupingEnabled);
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
                OperationHistory += $"{_calculatorEngine.FormatNumber(double.Parse(value), _digitGroupingEnabled)} {operation} ";
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
                MessageBox.Show("Error", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void ClearEntry()
        {
            UpdateDisplay(_calculatorEngine.ClearEntry());
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
