using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Globalization; // Adaugă acest using

namespace Tema1Calculator
{
    public class CalculatorEngine
    {
        private double _currentValue;
        private double _storedValue;
        private string _currentOperation;
        private bool _isNewNumber;
        private double _memory;
        private bool _hasDecimalPoint;

        public CalculatorEngine()
        {
            Reset();
        }

        public void Reset()
        {
            _currentValue = 0;
            _storedValue = 0;
            _currentOperation = "";
            _isNewNumber = true;
            _memory = 0;
            _hasDecimalPoint = false;
        }

        public double CurrentValue => _currentValue;
        public string CurrentOperation => _currentOperation;


        public void ValidateAndEnterDigit(string digit)
        {
            if (!Regex.IsMatch(digit, @"^[0-9.]$"))
                throw new ArgumentException("Invalid input: Only digits and decimal point allowed");

            // If starting a new number, reset current value
            if (_isNewNumber)
            {
                _currentValue = 0;
                _isNewNumber = false;
                _hasDecimalPoint = false;
            }

            if (digit == ".")
            {
                if (_hasDecimalPoint)
                    return; // Already has decimal point

                _hasDecimalPoint = true;
                return;
            }

            // For regular digits
            if (!_hasDecimalPoint)
            {
                // For whole number part
                _currentValue = _currentValue * 10 + double.Parse(digit);
            }
            else
            {
                // For decimal part
                string currentValueStr = _currentValue.ToString(CultureInfo.InvariantCulture);
                _currentValue = double.Parse(currentValueStr + digit, CultureInfo.InvariantCulture);
            }
        }
        public double SetOperation(string operation)
        {
            string[] validOperations = { "+", "-", "*", "/" };
            if (Array.IndexOf(validOperations, operation) == -1)
                throw new ArgumentException("Invalid operation");

            // If there's a pending operation, calculate it first
            if (!string.IsNullOrEmpty(_currentOperation))
            {
                try
                {
                    _currentValue = Calculate();
                }
                catch (Exception)
                {
                    // Keep current value if calculation fails
                }
            }

            // Save the current value and setup for next input
            _storedValue = _currentValue;
            _currentOperation = operation;
            _isNewNumber = true;
            _hasDecimalPoint = false;

            return _storedValue;
        }
        public double Calculate()
        {
            if (string.IsNullOrEmpty(_currentOperation))
                return _currentValue;

            try
            {
                switch (_currentOperation)
                {
                    case "+":
                        _currentValue = _storedValue + _currentValue;
                        break;
                    case "-":
                        _currentValue = _storedValue - _currentValue;
                        break;
                    case "*":
                        _currentValue = _storedValue * _currentValue;
                        break;
                    case "/":
                        if (_currentValue == 0)
                            throw new DivideByZeroException("Cannot divide by zero");

                        _currentValue = _storedValue / _currentValue;
                        break;
                }

                _storedValue = _currentValue;
                _currentOperation = "";
                _isNewNumber = true;
                _hasDecimalPoint = false;

                return _currentValue;
            }
            catch (Exception)
            {
                Reset();
                throw;
            }
        }

        // Funcții pentru operații speciale

        public double Backspace()
        {
            if (_isNewNumber)
                return 0;

            string currentValueStr = _currentValue.ToString();
            if (currentValueStr.Length <= 1)
            {
                _currentValue = 0;
                _isNewNumber = true;
                _hasDecimalPoint = false;
            }
            else
            {
                // Remove last character
                string newValueStr = currentValueStr.Substring(0, currentValueStr.Length - 1);
                _currentValue = double.Parse(newValueStr);
                _hasDecimalPoint = newValueStr.Contains(".");
            }

            return _currentValue;
        }

        public double ClearEntry()
        {
            _currentValue = 0;
            _isNewNumber = true;
            _hasDecimalPoint = false;
            return _currentValue;
        }


        public double ChangeSign()
        {
            _currentValue = -_currentValue;
            return _currentValue;
        }

        public double Reciprocal()
        {
            if (_currentValue == 0)
                throw new DivideByZeroException("Cannot calculate reciprocal of zero");

            _currentValue = 1 / _currentValue;
            return _currentValue;
        }

        public double Square()
        {
            _currentValue = Math.Pow(_currentValue, 2);
            return _currentValue;
        }

        public double SquareRoot()
        {
            if (_currentValue < 0)
                throw new ArithmeticException("Cannot calculate square root of negative number");

            _currentValue = Math.Sqrt(_currentValue);
            return _currentValue;
        }

        public double Percentage()
        {
            if (string.IsNullOrEmpty(_currentOperation))
                return _currentValue;

            switch (_currentOperation)
            {
                case "+":
                    _currentValue = _storedValue + (_storedValue * (_currentValue / 100));
                    break;
                case "-":
                    _currentValue = _storedValue - (_storedValue * (_currentValue / 100));
                    break;
                case "*":
                    _currentValue = _storedValue * (_currentValue / 100);
                    break;
                case "/":
                    _currentValue = _storedValue / (_currentValue / 100);
                    break;
            }

            _storedValue = _currentValue;
            _isNewNumber = true;
            _hasDecimalPoint = false;
            return _currentValue;
        }

        // Funcții de memorie
        private List<double> _memoryList = new List<double>();

        public void MemoryStore()
        {
            _memory = _currentValue;
            _memoryList.Add(_memory);
        }

        public List<double> GetMemoryList()
        {
            return new List<double>(_memoryList);
        }

        public void ClearMemoryList()
        {
            _memoryList.Clear();
            _memory = 0;
        }

        public double RecallMemory()
        {
            return _memory; 
        }

        public void UseValueFromMemory(double value)
        {
            _currentValue = value;
            _isNewNumber = true;
        }

        public void MemoryAdd()
        {
            if (_memoryList.Count > 0)
            {
                int lastIndex = _memoryList.Count - 1;
                double lastValue = _memoryList[lastIndex];

                _memoryList[lastIndex] = lastValue + _currentValue;

                _memory = _memoryList[lastIndex];
            }
            else
            {
                _memory = _currentValue;
                _memoryList.Add(_memory);
            }
        }

        public void MemorySubtract()
        {
            if (_memoryList.Count > 0)
            {
                int lastIndex = _memoryList.Count - 1;
                double lastValue = _memoryList[lastIndex];

                _memoryList[lastIndex] = lastValue - _currentValue;

                _memory = _memoryList[lastIndex];
            }
            else
            {
                _memory = -_currentValue;
                _memoryList.Add(_memory);
            }
        }
    }
}
