using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using VSTalk.Engine.Model;
using VSTalk.Model;

namespace VSTalk.Engine.Converters
{
    public class InterlocutorStateToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var state = (ContactState) value;
            switch (state)
            {
                case ContactState.Online:
                    return Brushes.LightGreen;
                case ContactState.Offline:
                    return Brushes.DarkGray;
                case ContactState.Away:
                    return Brushes.GreenYellow;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}