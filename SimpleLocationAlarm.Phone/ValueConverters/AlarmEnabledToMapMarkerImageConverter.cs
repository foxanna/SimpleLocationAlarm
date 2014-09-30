using System;
using Windows.UI.Xaml.Data;

namespace SimpleLocationAlarm.Phone.ValueConverters
{
    public class AlarmEnabledToMapMarkerImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? "../Assets/marker_violet.png" : "../Assets/marker_grey.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
