using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Simocracy.CLSim.GUI
{
    /// <summary>
    /// Converter for Window Title
    /// </summary>
    public class TitleConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            String subtitle1 = values[0].ToString();
            String subtitle2 = values[1].ToString();
            return $"{subtitle1} - {subtitle2}";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

    }

    /// <summary>
    /// Converts the given text into a visibility for notes in empty text boxes
    /// </summary>
    /// <seealso cref="https://code.msdn.microsoft.com/windowsapps/How-to-add-a-hint-text-to-ed66a3c6"/>
    public class TextInputToVisibilityConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Always test MultiValueConverter inputs for non-null 
            // (to avoid crash bugs for views in the designer) 
            if(values[0] is bool && values[1] is bool)
            {
                bool hasText = !(bool)values[0];
                bool hasFocus = (bool)values[1];
                if(hasFocus || hasText)
                    return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

    /// <summary>
    /// Converts an expanded property into bool
    /// </summary>
    public class ExpanderToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(System.Convert.ToBoolean(value))
                return parameter;
            return null;
        }
    }
}
