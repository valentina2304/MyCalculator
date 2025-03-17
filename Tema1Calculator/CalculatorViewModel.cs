using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Tema1Calculator
{
    public class CalculatorViewModel : INotifyPropertyChanged
    {
        private readonly CalculatorEngine _calculatorEngine;
        private string _displayText;
        private string _operationHistory;
        private bool _digitGroupingEnabled;
        private string _calculatorMode;
        private int _programmerBase;


        public event PropertyChangedEventHandler PropertyChanged;

        public CalculatorViewModel()
        {
            _calculatorEngine = new CalculatorEngine();
            _displayText = "0";
            _operationHistory = "";

            _digitGroupingEnabled = SettingsManager.DigitGroupingEnabled;
            _calculatorMode = SettingsManager.CalculatorMode;
            _programmerBase = SettingsManager.ProgrammerBase;
        }

        public string CalculatorMode
        {
            get => _calculatorMode;
            set
            {
                _calculatorMode = value;
                SettingsManager.CalculatorMode = value;
                OnPropertyChanged();
            }
        }

        public int ProgrammerBase
        {
            get => _programmerBase;
            set
            {
                _programmerBase = value;
                SettingsManager.ProgrammerBase = value;
                OnPropertyChanged();
                if (CalculatorMode == "Programmer")
                {
                    UpdateDisplay(_calculatorEngine.CurrentValue);
                }
            }
        }

        private string _hexValue;
        public string HexValue
        {
            get => _hexValue;
            set
            {
                _hexValue = value;
                OnPropertyChanged();
            }
        }

        private string _decValue;
        public string DecValue
        {
            get => _decValue;
            set
            {
                _decValue = value;
                OnPropertyChanged();
            }
        }

        private string _octValue;
        public string OctValue
        {
            get => _octValue;
            set
            {
                _octValue = value;
                OnPropertyChanged();
            }
        }

        private string _binValue;
        public string BinValue
        {
            get => _binValue;
            set
            {
                _binValue = value;
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
                SettingsManager.DigitGroupingEnabled = value;
                UpdateDisplay(_calculatorEngine.CurrentValue);
            }
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

        public void EnterDigitProgrammer(string digit)
        {
            try
            {
                // Convert the digit to decimal based on the current base
                int digitValue;

                if (digit.Length == 1 && "ABCDEF".Contains(digit.ToUpper()))
                {
                    if (ProgrammerBase < 16)
                        return; // Ignore hex digits in non-hex modes

                    digitValue = "ABCDEF".IndexOf(digit.ToUpper()) + 10;
                }
                else
                {
                    if (!int.TryParse(digit, out digitValue))
                        return;

                    // Check if the digit is valid for the current base
                    if (digitValue >= ProgrammerBase)
                        return;
                }

                // Calculate the actual value based on current base
                double currentValue = _calculatorEngine.CurrentValue;

                // All calculations internally use base 10
                _calculatorEngine.ValidateAndEnterDigit(digitValue.ToString());

                // Update display in the selected base
                UpdateDisplayForProgrammerMode(_calculatorEngine.CurrentValue);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void SetOperation(string operation)
        {
            try
            {
                string result = _calculatorEngine.SetOperation(operation).ToString();
                if (CalculatorMode == "Programmer")
                {
                    // Update all displays for programmer mode
                    UpdateDisplayForProgrammerMode(_calculatorEngine.CurrentValue);

                    // Update operation history
                    OperationHistory += $"{_calculatorEngine.FormatNumberInBase(_calculatorEngine.CurrentValue, _programmerBase)} {operation} ";
                }
                else
                {
                    UpdateDisplayWithHistory(result, operation);
                }
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
                double result = _calculatorEngine.Calculate();

                if (CalculatorMode == "Programmer")
                {
                    // Update all displays for programmer mode
                    UpdateDisplayForProgrammerMode(result);
                    OperationHistory = "";
                }
                else
                {
                    UpdateDisplayWithHistory(result.ToString(), "=");
                }
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
            if (CalculatorMode == "Programmer")
            {
                UpdateDisplayForProgrammerMode(value);
            }
            else
            {
                DisplayText = _calculatorEngine.FormatNumber(value, _digitGroupingEnabled);
            }
        }


        private void UpdateDisplayForProgrammerMode(double value)
        {
            // Ensure we're working with integer values for base conversion
            long intValue = (long)value;

            // Update values in different bases
            HexValue = Convert.ToString(intValue, 16).ToUpper();
            DecValue = intValue.ToString();
            OctValue = Convert.ToString(intValue, 8);
            BinValue = Convert.ToString(intValue, 2);

            // Update the main display based on selected base
            DisplayText = _calculatorEngine.FormatNumberInBase(value, ProgrammerBase);
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
