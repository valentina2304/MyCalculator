using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tema1Calculator
{
    public class CalculatorEngine
    {
        private NumberProcessor _processor;
        private MemoryManager _memoryManager;
        private AdvancedOperations _advancedOps;
        private DisplayFormatter _formatter;
        private bool _digitGroupingEnabeld;

        public CalculatorEngine()
        {
            _processor = new NumberProcessor();
            _memoryManager = new MemoryManager();
            _advancedOps = new AdvancedOperations();
            _formatter = new DisplayFormatter();
        }

        public double CurrentValue => _processor.CurrentValue;
        public string CurrentOperation => _processor.CurrentOperation;

        public void Reset()
        {
            _processor.Reset();
        }

        public void ValidateAndEnterDigit(string digit)
        {
            _processor.ValidateAndEnterDigit(digit);
        }

        public double SetOperation(string operation)
        {
            return _processor.SetOperation(operation);
        }

        public double Calculate()
        {
            return _processor.Calculate();
        }

        public double Backspace()
        {
            bool isNewNumber = _processor.IsNewNumber;
            double result = _advancedOps.Backspace(CurrentValue, ref isNewNumber);
            if (isNewNumber)
                _processor.Reset();
            else
                _processor.SetValue(result);
            return result;
        }

        public double ClearEntry()
        {
            double result = _advancedOps.ClearEntry();
            _processor.SetValue(result);
            return result;
        }

        public double ChangeSign()
        {
            double result = _advancedOps.ChangeSign(CurrentValue);
            _processor.SetValue(result);
            return result;
        }

        public double Reciprocal()
        {
            double result = _advancedOps.Reciprocal(CurrentValue);
            _processor.SetValue(result);
            return result;
        }

        public double Square()
        {
            double result = _advancedOps.Square(CurrentValue);
            _processor.SetValue(result);
            return result;
        }

        public double SquareRoot()
        {
            double result = _advancedOps.SquareRoot(CurrentValue);
            _processor.SetValue(result);
            return result;
        }

        public double Percentage()
        {

            double result = _advancedOps.Percentage(CurrentValue, _processor.StoredValue, CurrentOperation);
            _processor.SetValue(result);
            return result;
        }

        // Funcții pentru formatter
        public string FormatNumber(double value, bool useDigitGrouping)
        {
            return _formatter.FormatNumber(value, useDigitGrouping);
        }

        public string FormatNumberInBase(double value, int numberBase)
        {
            if (value != Math.Floor(value) || value < 0)
            {
                // Non-integers or negative numbers can only be displayed in base 10
                return value.ToString();
            }

            long longValue = (long)value;
            switch (numberBase)
            {
                case 2:
                    return Convert.ToString(longValue, 2);
                case 8:
                    return Convert.ToString(longValue, 8);
                case 10:
                    return FormatNumber(value, _digitGroupingEnabeld);
                case 16:
                    return Convert.ToString(longValue, 16).ToUpper();
                default:
                    return value.ToString();
            }
        }


        // Funcții pentru memoria calculatorului
        public void MemoryStore()
        {
            _memoryManager.MemoryStore(CurrentValue);
        }

        public void MemoryAdd()
        {
            _memoryManager.MemoryAdd(CurrentValue);
        }

        public void MemorySubtract()
        {
            _memoryManager.MemorySubtract(CurrentValue);
        }

        public double RecallMemory()
        {
            double value = _memoryManager.RecallMemory();
            _processor.SetValue(value);
            return value;
        }

        public void ClearMemoryList()
        {
            _memoryManager.ClearMemoryList();
        }

        public List<double> GetMemoryList()
        {
            return _memoryManager.GetMemoryList();
        }

        public void UseValueFromMemory(double value)
        {
            _processor.SetValue(value);
        }


    }
}
