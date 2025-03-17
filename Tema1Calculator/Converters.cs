using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Tema1Calculator
{
    public class ModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString() == parameter.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter.ToString();
        }
    }

    public class BaseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int currentBase = (int)value;
            int targetBase = int.Parse(parameter.ToString());
            return currentBase == targetBase;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return int.Parse(parameter.ToString());
        }
    }

    public class ModeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string mode = value.ToString();
            string targetMode = parameter.ToString();
            return mode == targetMode ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
