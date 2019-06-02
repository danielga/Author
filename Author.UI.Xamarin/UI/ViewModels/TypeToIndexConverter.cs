using System;
using System.Globalization;
using Xamarin.Forms;

namespace Author.UI.ViewModels
{
    public class TypeToIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((OTP.Type)value).Value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return OTP.Type.Parse((int)value);
        }
    }
}
