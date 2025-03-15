using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tema1Calculator
{
    class AdvancedOperations
    {
        public double Backspace(double currentValue, ref bool isNewNumber)
        {
            if (isNewNumber)
                return 0;

            string currentValueStr = currentValue.ToString();
            if (currentValueStr.Length <= 1)
            {
                currentValue = 0;
                isNewNumber = true;
                return currentValue;
            }
            else
            {
                string newValueStr = currentValueStr.Substring(0, currentValueStr.Length - 1);
                return double.Parse(newValueStr);
            }
        }

        public double ClearEntry()
        {
            return 0;
        }

        public double ChangeSign(double currentValue)
        {
            return -currentValue;
        }

        public double Reciprocal(double currentValue)
        {
            if (currentValue == 0)
                throw new DivideByZeroException("Cannot calculate reciprocal of zero");

            return 1 / currentValue;
        }

        public double Square(double currentValue)
        {
            return Math.Pow(currentValue, 2);
        }

        public double SquareRoot(double currentValue)
        {
            if (currentValue < 0)
                throw new ArithmeticException("Cannot calculate square root of negative number");

            return Math.Sqrt(currentValue);
        }

        public double Percentage(double currentValue, double storedValue, string operation)
        {
            if (string.IsNullOrEmpty(operation))
                return currentValue;

            switch (operation)
            {
                case "+":
                    return storedValue + (storedValue * (currentValue / 100));
                case "-":
                    return storedValue - (storedValue * (currentValue / 100));
                case "*":
                    return storedValue * (currentValue / 100);
                case "/":
                    if (currentValue == 0)
                        throw new DivideByZeroException("Cannot divide by zero");
                    return storedValue / (currentValue / 100);
                default:
                    return currentValue;
            }
        }
    }
}
