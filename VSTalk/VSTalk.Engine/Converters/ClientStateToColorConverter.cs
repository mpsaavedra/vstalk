using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using VSTalk.Engine.Model;
using VSTalk.Model;

namespace VSTalk.Engine.Converters
{
    public class ClientStateToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var state = (ClientState)value;
            switch (state)
            {
                case ClientState.Connected:
                    return Brushes.Green;
                case ClientState.Connecting:
                    return Brushes.Orange;
                case ClientState.Disconnected:
                    return Brushes.DarkGray;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}