using System;
using System.Globalization;
using System.Windows.Data;
using WpfProductionLineApp.ViewModels;

namespace WpfProductionLineApp.Converters
{
    public class SuckStateToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            RobotSuckState state = (RobotSuckState)value;

            if (state == RobotSuckState.Disconnect)
                return @"\Images\Gray.jpg";
            else if (state == RobotSuckState.open)
                return @"\Images\Green.jpg";
            else
                return @"\Images\Red.jpg";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
