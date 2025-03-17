using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tema1Calculator
{
    class NumberProcessor
    {
        private double _currentValue;
        private double _storedValue;
        private string _currentOperation;
        private bool _isNewNumber;
        private bool _hasDecimalPoint;

        public NumberProcessor()
        {
            Reset();
        }

        public double CurrentValue => _currentValue;
        public string CurrentOperation => _currentOperation;
        public bool IsNewNumber => _isNewNumber;

        public bool HasDecimalPoint => _hasDecimalPoint;
        public double StoredValue => _storedValue;

        public void Reset()
        {
            _currentValue = 0;
            _storedValue = 0;
            _currentOperation = "";
            _isNewNumber = true;
            _hasDecimalPoint = false;
        }

        public void ValidateAndEnterDigit(string digit)
        {
            string decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            if (_isNewNumber)
            {
                _currentValue = 0;
                _isNewNumber = false;
                _hasDecimalPoint = false;
            }

            if (digit == decimalSeparator)
            {
                if (_hasDecimalPoint)
                    return;

                _hasDecimalPoint = true;
                return;
            }

            int digitValue;

            if (digit.Length == 1 && "ABCDEF".Contains(digit.ToUpper()))
            {
                digitValue = "ABCDEF".IndexOf(digit.ToUpper()) + 10;
            }
            else
            {
                
                if (!int.TryParse(digit, out digitValue))
                    return;
            }

            if (!_hasDecimalPoint)
            {
                _currentValue = _currentValue * 10 + digitValue;
            }
            else
            {
                string currentValueStr = _currentValue.ToString(CultureInfo.CurrentCulture);
                if (!currentValueStr.Contains(decimalSeparator))
                    currentValueStr += decimalSeparator;

                _currentValue = double.Parse(currentValueStr + digit, CultureInfo.CurrentCulture);
            }
        }
        public double SetOperation(string operation)
        {
            string[] validOperations = { "+", "-", "*", "/" };
            if (Array.IndexOf(validOperations, operation) == -1)
                throw new ArgumentException("Invalid operation");

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

        public double SetValue(double value)
        {
            _currentValue = value;
            _isNewNumber = true;
            return _currentValue;
        }
    }
}
