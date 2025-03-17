using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Tema1Calculator
{
    public class ExpressionEvaluator
    {
        private readonly List<string> _operators = new List<string> { "+", "-", "*", "/" };
        private readonly List<double> _values = new List<double>();
        private readonly List<string> _operations = new List<string>();
        private double _currentValue;
        private bool _hasCurrentValue;

        public ExpressionEvaluator()
        {
            Reset();
        }

        public void Reset()
        {
            _values.Clear();
            _operations.Clear();
            _currentValue = 0;
            _hasCurrentValue = false;
        }

        public void AddValue(double value)
        {
            if (_hasCurrentValue)
            {
                _values.Add(_currentValue);
                _currentValue = value;
            }
            else
            {
                _currentValue = value;
                _hasCurrentValue = true;
            }
        }

        public void AddOperation(string operation)
        {
            if (!_operators.Contains(operation))
                throw new ArgumentException("Invalid operation");

            if (_hasCurrentValue)
            {
                _values.Add(_currentValue);
                _operations.Add(operation);
                _hasCurrentValue = false;
            }
            else if (_values.Count > 0)
            {
                _operations[_operations.Count - 1] = operation;
            }
        }

        public double Evaluate()
        {
            if (_hasCurrentValue)
            {
                _values.Add(_currentValue);
                _hasCurrentValue = false;
            }

            if (_values.Count == 0)
                return 0;

            if (_values.Count == 1)
                return _values[0];

            if (_values.Count != _operations.Count + 1)
                throw new InvalidOperationException("Invalid expression format");

            List<double> values = new List<double>(_values);
            List<string> operations = new List<string>(_operations);

            for (int i = 0; i < operations.Count; i++)
            {
                if (operations[i] == "*" || operations[i] == "/")
                {
                    double result;
                    if (operations[i] == "*")
                    {
                        result = values[i] * values[i + 1];
                    }
                    else 
                    {
                        if (values[i + 1] == 0)
                            throw new DivideByZeroException("Cannot divide by zero");
                        result = values[i] / values[i + 1];
                    }

                    values[i] = result;
                    values.RemoveAt(i + 1);
                    operations.RemoveAt(i);
                    i--; 
                }
            }

            for (int i = 0; i < operations.Count; i++)
            {
                double result;
                if (operations[i] == "+")
                {
                    result = values[i] + values[i + 1];
                }
                else 
                {
                    result = values[i] - values[i + 1];
                }

                values[i] = result;
                values.RemoveAt(i + 1);
                operations.RemoveAt(i);
                i--; 
            }

            return values[0];
        }

        public string GetExpressionString()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < _values.Count; i++)
            {
                sb.Append(_values[i].ToString(CultureInfo.InvariantCulture));
                if (i < _operations.Count)
                {
                    sb.Append(" ").Append(_operations[i]).Append(" ");
                }
            }

            if (_hasCurrentValue)
            {
                if (_values.Count > 0)
                {
                    sb.Append(" ").Append(_operations.LastOrDefault()).Append(" ");
                }
                sb.Append(_currentValue.ToString(CultureInfo.InvariantCulture));
            }

            return sb.ToString();
        }
    }
}