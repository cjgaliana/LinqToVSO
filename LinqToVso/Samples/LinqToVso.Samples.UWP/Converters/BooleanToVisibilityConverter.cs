using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace LinqToVso.Samples.UWP.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var isVisible = (bool) value;
            if (parameter != null && string.Equals("Invert", parameter.ToString()))
            {
                isVisible = !isVisible;
            }
            return isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}