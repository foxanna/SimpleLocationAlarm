using System;
using Android.Gms.Common;
using Android.Widget;
using Android.App;

namespace SimpleLocationAlarm.Droid.Screens
{
	public partial class HomeActivity
	{
		const int _googleServicesCheckRequestCode = 25;
		int _isGooglePlayServicesAvailable;

		void CheckGS ()
		{
			_isGooglePlayServicesAvailable = GooglePlayServicesUtil.IsGooglePlayServicesAvailable (this);
			if (_isGooglePlayServicesAvailable != ConnectionResult.Success) {
				if (GooglePlayServicesUtil.IsUserRecoverableError (_isGooglePlayServicesAvailable)) {
					GooglePlayServicesUtil.ShowErrorDialogFragment (_isGooglePlayServicesAvailable, this, _googleServicesCheckRequestCode);
				} else {
					Toast.MakeText (this, Resource.String.device_not_supported, 
						ToastLength.Long).Show ();
					Finish ();
				}
			}
		}

		void OnActivityResultForGS (Result resultCode)
		{
			if (resultCode == Result.Canceled) {
				GooglePlayServicesUtil.ShowErrorNotification (_isGooglePlayServicesAvailable, this);
				Finish ();
			}
		}
	}
}

