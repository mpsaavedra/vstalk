using System;
using System.Globalization;
using System.Windows.Data;
using VSTalk.Engine.Model;
using VSTalk.Model;

namespace VSTalk.Engine.Converters
{
    public class CommonStateToActionNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var commonState = (ClientState) value;
            switch (commonState)
            {
                case ClientState.Connected:
                    return "Disconect";
                case ClientState.Connecting:
                    return "Connecting";
                case ClientState.Disconnected:
                    return "Connect";
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}