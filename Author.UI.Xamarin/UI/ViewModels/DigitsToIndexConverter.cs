using System;
using System.Globalization;
using Xamarin.Forms;

namespace Author.UI.ViewModels
{
    public class DigitsToIndexConverter : IValueConverter
    {
        private const int PasswordLengthDifference = 4;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (byte)value - PasswordLengthDifference;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value + PasswordLengthDifference;
        }
    }
}
