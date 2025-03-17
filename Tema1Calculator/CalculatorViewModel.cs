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
        private bool _usePrecedenceEnabled;

        public event PropertyChangedEventHandler PropertyChanged;

        public CalculatorViewModel()
        {
            _calculatorEngine = new CalculatorEngine();
            _displayText = "0";
            _operationHistory = "";
            _calculatorEngine.UsePrecedence = _usePrecedenceEnabled;


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

        public bool UsePrecedenceEnabled
        {
            get => _usePrecedenceEnabled;
            set
            {
                _usePrecedenceEnabled = value;
                OnPropertyChanged();
                // SettingsManager.UsePrecedenceEnabled = value;
                _calculatorEngine.UsePrecedence = value;

                Clear();
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
                ShowErrorMessage("Overflow");
            }
            catch (ArgumentException)
            {
                ShowErrorMessage("Invalid Input");
            }
        }

        public void EnterDigitProgrammer(string digit)
        {
            try
            {
                if (!IsValidDigitForCurrentBase(digit))
                    return;

                int digitValue = GetDigitValue(digit);
                _calculatorEngine.ValidateAndEnterDigit(digitValue.ToString());

                UpdateDisplayForProgrammerMode(_calculatorEngine.CurrentValue);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private bool IsValidDigitForCurrentBase(string digit)
        {
            if (digit.Length == 1 && "ABCDEF".Contains(digit.ToUpper()))
            {
                return ProgrammerBase >= 16; 
            }

            if (!int.TryParse(digit, out int digitValue))
            {
                return false;
            }

            return digitValue < ProgrammerBase;
        }

        private int GetDigitValue(string digit)
        {
            if (digit.Length == 1 && "ABCDEF".Contains(digit.ToUpper()))
            {
                return "ABCDEF".IndexOf(digit.ToUpper()) + 10;
            }

            return int.Parse(digit);
        }

        public void SetOperation(string operation)
        {
            try
            {
                double value = _calculatorEngine.SetOperation(operation);

                if (CalculatorMode == "Programmer")
                {
                    UpdateDisplayForProgrammerMode(_calculatorEngine.CurrentValue);

                    OperationHistory += $"{_calculatorEngine.FormatNumberInBase(_calculatorEngine.CurrentValue, _programmerBase)} {operation} ";
                }
                else
                {
                    UpdateDisplayWithHistory(value.ToString(), operation);
                }
            }
            catch (ArgumentException)
            {
                ShowErrorMessage("Invalid Operation");
            }
        }

        public void Calculate()
        {
            try
            {
                double result = _calculatorEngine.Calculate();

                if (CalculatorMode == "Programmer")
                {
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
                ShowErrorMessage("Divide by Zero");
            }
            catch (Exception)
            {
                ShowErrorMessage("Error");
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
            long intValue = (long)value;

            HexValue = Convert.ToString(intValue, 16).ToUpper();
            DecValue = intValue.ToString();
            OctValue = Convert.ToString(intValue, 8);
            BinValue = Convert.ToString(intValue, 2);

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

        // Helper method for executing operations and handling errors uniformly
        private void ExecuteOperation(Func<double> operation, string errorMessage = "Error")
        {
            try
            {
                UpdateDisplay(operation());
            }
            catch (DivideByZeroException)
            {
                ShowErrorMessage("Divide by Zero");
            }
            catch (ArithmeticException)
            {
                ShowErrorMessage("Invalid Input");
            }
            catch (Exception)
            {
                ShowErrorMessage(errorMessage);
            }
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        // Funcții pentru operații speciale
        public void Backspace()
        {
            ExecuteOperation(() => _calculatorEngine.Backspace());
        }

        public void ClearEntry()
        {
            UpdateDisplay(_calculatorEngine.ClearEntry());
        }

        public void ChangeSign()
        {
            ExecuteOperation(() => _calculatorEngine.ChangeSign());
        }

        public void Reciprocal()
        {
            ExecuteOperation(() => _calculatorEngine.Reciprocal(), "Divide by Zero");
        }

        public void Square()
        {
            ExecuteOperation(() => _calculatorEngine.Square());
        }

        public void SquareRoot()
        {
            ExecuteOperation(() => _calculatorEngine.SquareRoot(), "Invalid Input");
        }

        public void Percentage()
        {
            ExecuteOperation(() => _calculatorEngine.Percentage());
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