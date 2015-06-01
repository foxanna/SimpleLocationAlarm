using System;
using System.Linq;
using Xamarin.InAppBilling;
using Xamarin.InAppBilling.Utilities;
using System.Collections.Generic;
using Android.App;
using Android.Content;

namespace SimpleLocationAlarm.Droid.Services
{
	public class InAppBillingManager
	{
		InAppBillingServiceConnection _serviceConnection;

		string _productId;

		public event EventHandler HasPaiedChanged, Connected;

		bool hasPaied = true;

		public bool HasPaied { 
			get { return hasPaied; } 
			private set {
				hasPaied = value;
				var handler = HasPaiedChanged;
				if (handler != null)
					handler (this, EventArgs.Empty);
			}
		}

		public InAppBillingManager (Activity activity)
		{
			string value = Security.Unify (activity.Resources.GetStringArray (Resource.Array.billing_key), new int[] {
				0,
				1,
				2,
				3
			});

			_productId = activity.Resources.GetString (Resource.String.ads_product_id);

			_serviceConnection = new InAppBillingServiceConnection (activity, value);
			if (_serviceConnection != null) {
				_serviceConnection.OnConnected += OnConnected;
				_serviceConnection.Connect ();
			}
		}

		void OnConnected ()
		{
			var handler = Connected;
			if (handler != null)
				handler (this, EventArgs.Empty);
		}

		public void CheckIfAdsFreeWasBought ()
		{
			var purchases = _serviceConnection.BillingHandler.GetPurchases (ItemType.Product);
			HasPaied = (purchases != null && purchases.Any ());

			if (HasPaied)
				try {
					bool result = _serviceConnection.BillingHandler.ConsumePurchase (purchases.First ());
				} catch {
				}
		}

		public void OnActivityResult (int requestCode, Result resultCode, Intent data)
		{
			_serviceConnection.BillingHandler.HandleActivityResult (requestCode, resultCode, data);
		}

		public async void Pay ()
		{
			_serviceConnection.BillingHandler.OnProductPurchased +=	BillingHandler_OnProductPurchaseCompleted;

			var _products = await _serviceConnection.BillingHandler.QueryInventoryAsync (new List<string> {
				_productId
			}, ItemType.Product);

			if (_products != null && _products.Any ())
				_serviceConnection.BillingHandler.BuyProduct (_products.First ());
		}

		void BillingHandler_OnProductPurchaseCompleted (int response, Purchase purchase, string purchaseData, string purchaseSignature)
		{
			CheckIfAdsFreeWasBought ();
		}
	}
}

