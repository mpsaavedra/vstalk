using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using VSTalk.Model;

namespace VSTalk.Engine.Converters
{
    public class VisibleIfDisconected : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var state = (ClientState) value;
            return state == ClientState.Disconnected ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}