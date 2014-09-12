using System.Collections.Generic;
using System.Linq;

using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Preferences;
using Android.Util;
using Android.Content.Res;

namespace SimpleLocationAlarm.Droid.Screens
{
    public class NumberPickerPreference : DialogPreference 
    {
        public NumberPickerPreference(Context context, IAttributeSet attrs) : base (context, attrs)
        {
            DialogLayoutResource = Resource.Layout.NumberPickerDialog;            

            SetPositiveButtonText(Android.Resource.String.Ok);
            SetNegativeButtonText(Android.Resource.String.Cancel);

            int value = PreferenceManager.GetDefaultSharedPreferences(Context).GetInt(SettingsScreen.DefaultRadiusKey, SettingsScreen.DefaultRadiusValue);
            Summary = string.Format(context.GetString(Resource.String.settings_default_radius_sum), value);
        }

        protected override void OnDialogClosed(bool positiveResult)
        {
            if (positiveResult)
            {
                PersistInt(int.Parse(_values[_numberPicker.Value]));
                Summary = string.Format(Context.GetString(Resource.String.settings_default_radius_sum), GetPersistedInt(SettingsScreen.DefaultRadiusValue));
            }

            base.OnDialogClosed(positiveResult);
        }
        
        NumberPicker _numberPicker;
        List<string> _values = new List<string>() { "50", "100", "150", "200", "300", "400", "500", "600", "700", "800", "900", "1000" };

        protected override void OnBindDialogView(View view)
        {
            base.OnBindDialogView(view);

            _numberPicker = (NumberPicker)view;
            _numberPicker.DescendantFocusability = DescendantFocusability.BlockDescendants;
            _numberPicker.SetDisplayedValues(_values.ToArray());
            _numberPicker.MinValue = 0;
            _numberPicker.MaxValue = _values.Count() - 1;
            _numberPicker.WrapSelectorWheel = false;

            _numberPicker.Value = _values.IndexOf(GetPersistedInt(SettingsScreen.DefaultRadiusValue).ToString());
        }
    }
}