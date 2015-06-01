using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace LocationAlarm.WindowsPhone.ValueConverters
{
    public class AlarmEnabledToMapMarkerTitleColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return new SolidColorBrush((bool)value ? Color.FromArgb(255, 153, 51, 204) : Color.FromArgb(255, 139, 139, 139)); 
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
