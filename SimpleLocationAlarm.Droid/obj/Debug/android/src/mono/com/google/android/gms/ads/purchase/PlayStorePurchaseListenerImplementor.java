package mono.com.google.android.gms.ads.purchase;


public class PlayStorePurchaseListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.google.android.gms.ads.purchase.PlayStorePurchaseListener
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_isValidPurchase:(Ljava/lang/String;)Z:GetIsValidPurchase_Ljava_lang_String_Handler:Android.Gms.Ads.purchase.IPlayStorePurchaseListenerInvoker, GooglePlayServicesLib\n" +
			"n_onInAppPurchaseFinished:(Lcom/google/android/gms/ads/purchase/InAppPurchaseResult;)V:GetOnInAppPurchaseFinished_Lcom_google_android_gms_ads_purchase_InAppPurchaseResult_Handler:Android.Gms.Ads.purchase.IPlayStorePurchaseListenerInvoker, GooglePlayServicesLib\n" +
			"";
		mono.android.Runtime.register ("Android.Gms.Ads.purchase.IPlayStorePurchaseListenerImplementor, GooglePlayServicesLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", PlayStorePurchaseListenerImplementor.class, __md_methods);
	}


	public PlayStorePurchaseListenerImplementor () throws java.lang.Throwable
	{
		super ();
		if (getClass () == PlayStorePurchaseListenerImplementor.class)
			mono.android.TypeManager.Activate ("Android.Gms.Ads.purchase.IPlayStorePurchaseListenerImplementor, GooglePlayServicesLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public boolean isValidPurchase (java.lang.String p0)
	{
		return n_isValidPurchase (p0);
	}

	private native boolean n_isValidPurchase (java.lang.String p0);


	public void onInAppPurchaseFinished (com.google.android.gms.ads.purchase.InAppPurchaseResult p0)
	{
		n_onInAppPurchaseFinished (p0);
	}

	private native void n_onInAppPurchaseFinished (com.google.android.gms.ads.purchase.InAppPurchaseResult p0);

	java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
