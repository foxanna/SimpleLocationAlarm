using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

namespace SimpleLocationAlarm.Phone.ValueConverters
{
    public class AlarmEnabledToMenuEnabledItemConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            ResourceLoader resourceLoader = new ResourceLoader();
            return resourceLoader.GetString( (bool)value ? "DisableAlarm" : "EnableAlarm");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
