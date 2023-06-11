using System;
using System.Globalization;
using System.Windows.Data;
using WpfProductionLineApp.Models;

namespace WpfProductionLineApp.Converters
{
    public class StringToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ConnectState image = (ConnectState)value;
            switch (image)
            {
                case ConnectState.Connected:
                    return @"\Images\Green.jpg";
                case ConnectState.Disconnected:
                    return @"\Images\Red.jpg";
                default:
                    throw new NotImplementedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
