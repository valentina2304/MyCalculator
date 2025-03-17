using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tema1Calculator
{
    class DisplayFormatter
    {
        public string FormatNumber(double value, bool useDigitGrouping)
        {
            string valueStr = value.ToString(CultureInfo.InvariantCulture);

            if (double.TryParse(valueStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double parsedValue))
            {
                if (useDigitGrouping)
                {
                    if (parsedValue == Math.Floor(parsedValue))
                    {
                        return parsedValue.ToString("N0", CultureInfo.CurrentCulture);
                    }
                    else
                    {
                        int decimalPlaces = 0;
                        int decimalIndex = valueStr.IndexOf('.');

                        if (decimalIndex >= 0)
                        {
                            decimalPlaces = valueStr.Length - decimalIndex - 1;
                        }

                        return parsedValue.ToString($"N{decimalPlaces}", CultureInfo.CurrentCulture);
                    }
                }
                else
                {
                    return valueStr;
                }
            }

            return valueStr;
        }

        public string FormatNumberInBase(double value, int numberBase, bool _digitGroupingEnabled)
        {
            if (value != Math.Floor(value) || value < 0)
            {
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
                    return FormatNumber(value, _digitGroupingEnabled);
                case 16:
                    return Convert.ToString(longValue, 16).ToUpper();
                default:
                    return value.ToString();
            }
        }
    }
}
