using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace LinqToVso.Samples.UWP.Converters
{
    public class ItemClickConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var click = (ItemClickEventArgs)value;
            return click.ClickedItem;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}