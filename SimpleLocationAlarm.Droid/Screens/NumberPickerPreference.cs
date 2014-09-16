using System.Collections.Generic;
using System.Linq;

using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Preferences;
using Android.Util;
using Android.Content.Res;
using Android.OS;
using Java.Lang.Reflect;
using Android.Graphics;
using Java.Lang;

namespace SimpleLocationAlarm.Droid.Screens
{
    public class NumberPickerPreference : DialogPreference 
    {
        public NumberPickerPreference(Context context, IAttributeSet attrs) : base (context, attrs)
        {
            DialogLayoutResource = (BuildVersionCodes.Honeycomb > Build.VERSION.SdkInt) ? Resource.Layout.RadioGroupDialog : Resource.Layout.NumberPickerDialog;            

            SetPositiveButtonText(Android.Resource.String.Ok);
            SetNegativeButtonText(Android.Resource.String.Cancel);

            int value = PreferenceManager.GetDefaultSharedPreferences(Context).GetInt(SettingsScreen.DefaultRadiusKey, SettingsScreen.DefaultRadiusValue);
            Summary = string.Format(context.GetString(Resource.String.settings_default_radius_sum), value);
        }

        protected override void OnDialogClosed(bool positiveResult)
        {
            if (positiveResult)
            {
                if (BuildVersionCodes.Honeycomb > Build.VERSION.SdkInt)
                {
                    PersistInt(_radioGroup.CheckedRadioButtonId);
                }
                else
                {
                    PersistInt(_values[_numberPicker.Value]);
                }
                    
                Summary = string.Format(Context.GetString(Resource.String.settings_default_radius_sum), GetPersistedInt(SettingsScreen.DefaultRadiusValue));                
            }

            base.OnDialogClosed(positiveResult);
        }
        
        NumberPicker _numberPicker;
        RadioGroup _radioGroup;

        List<int> _values = new List<int>() { 50, 100, 150, 200, 300, 400, 500, 600, 700, 800, 900, 1000 };

        protected override void OnBindDialogView(View view)
        {
            base.OnBindDialogView(view);

            var currentValue = GetPersistedInt(SettingsScreen.DefaultRadiusValue);

            if (BuildVersionCodes.Honeycomb > Build.VERSION.SdkInt)
            {
                _radioGroup = (RadioGroup)view;
                foreach (var distance in _values)
                {
                    var radioButton = new RadioButton(Context);
                    radioButton.Gravity = GravityFlags.Right | GravityFlags.CenterVertical;
                    radioButton.SetText(string.Format(Context.GetString(Resource.String.settings_default_radius_sum), distance), TextView.BufferType.Normal);
                    radioButton.SetTextColor(Context.Resources.GetColor(Resource.Color.dark));
                    radioButton.Checked = distance == currentValue;
                    radioButton.Id = distance;
                    _radioGroup.AddView(radioButton);
                }
            }
            else
            {
                _numberPicker = (NumberPicker)view;
                SetNumberPickerTextColor(_numberPicker, Context.Resources.GetColor(Resource.Color.dark));
                _numberPicker.DescendantFocusability = DescendantFocusability.BlockDescendants;
                _numberPicker.SetDisplayedValues(_values.Select(v=> v.ToString()).ToArray());
                _numberPicker.MinValue = 0;
                _numberPicker.MaxValue = _values.Count() - 1;
                _numberPicker.WrapSelectorWheel = false;
                _numberPicker.Value = _values.IndexOf(currentValue);
            }
        }

        static void SetNumberPickerTextColor(NumberPicker numberPicker, Color color)
        {
            try
            {
                int count = numberPicker.ChildCount;
                for (int i = 0; i < count; i++)
                {
                    View child = numberPicker.GetChildAt(i);
                    if (child is EditText)
                    {
                        try
                        {
                            ((EditText)child).SetTextColor(color);

                            Field selectorWheelPaintField = numberPicker.Class
                                .GetDeclaredField("mSelectorWheelPaint");
                            selectorWheelPaintField.Accessible = true;
                            ((Paint)selectorWheelPaintField.Get(numberPicker)).Color = color;
                            var a = selectorWheelPaintField.Get(numberPicker);
                        }
                        catch (NoSuchFieldException e)
                        {
                            Log.Debug("setNumberPickerTextColor", e.Message);
                        }
                        catch (IllegalAccessException e)
                        {
                            Log.Debug("setNumberPickerTextColor", e.Message);
                        }
                        catch (IllegalArgumentException e)
                        {
                            Log.Debug("setNumberPickerTextColor", e.Message);
                        }
                    }
                }

                numberPicker.Invalidate();
            }
            catch
            {

            }
        }
    }
}