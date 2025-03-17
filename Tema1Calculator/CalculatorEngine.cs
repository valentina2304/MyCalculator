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
        private ExpressionEvaluator _expressionEvaluator;
        private bool _digitGroupingEnabled;
        private bool _usePrecedence;

        public CalculatorEngine()
        {
            _processor = new NumberProcessor();
            _memoryManager = new MemoryManager();
            _advancedOps = new AdvancedOperations();
            _formatter = new DisplayFormatter();
            _expressionEvaluator = new ExpressionEvaluator();
            _usePrecedence = false;
            _digitGroupingEnabled = false;
        }

        public double CurrentValue => _processor.CurrentValue;
        public string CurrentOperation => _processor.CurrentOperation;
        public bool UsePrecedence
        {
            get => _usePrecedence;
            set => _usePrecedence = value;
        }

        public void Reset()
        {
            _processor.Reset();
            _expressionEvaluator.Reset();
        }

        public void ValidateAndEnterDigit(string digit)
        {
            _processor.ValidateAndEnterDigit(digit);
        }

        public double SetOperation(string operation)
        {
            if (!_usePrecedence)
            {
                return _processor.SetOperation(operation);
            }

            _expressionEvaluator.AddValue(_processor.CurrentValue);

            _expressionEvaluator.AddOperation(operation);

            double currentValue = _processor.CurrentValue;
            _processor.Reset();

            return currentValue;
        }

        public double Calculate()
        {
            if (!_usePrecedence)
            {
                return _processor.Calculate();
            }

            _expressionEvaluator.AddValue(_processor.CurrentValue);

            double result = _expressionEvaluator.Evaluate();

            _expressionEvaluator.Reset();
            _processor.SetValue(result);

            return result;
        }

        public string GetExpressionString()
        {
            return _expressionEvaluator.GetExpressionString();
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
            return _formatter.FormatNumberInBase(value, numberBase, _digitGroupingEnabled);
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