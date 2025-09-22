using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace U2.Logger.Frontend.Windows.Converters;

public class BooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool isVisible = false;
        if (value is bool boolValue)
        {
            isVisible = boolValue;
        }

        if (parameter != null && parameter.ToString() == "invert")
        {
            isVisible = !isVisible;
        }

        return isVisible ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}