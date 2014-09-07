package mono.com.google.ads.mediation;


public class MediationInterstitialListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.google.ads.mediation.MediationInterstitialListener
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onDismissScreen:(Lcom/google/ads/mediation/MediationInterstitialAdapter;)V:GetOnDismissScreen_Lcom_google_ads_mediation_MediationInterstitialAdapter_Handler:Google.Ads.Mediation.IMediationInterstitialListenerInvoker, GooglePlayServicesLib\n" +
			"n_onFailedToReceiveAd:(Lcom/google/ads/mediation/MediationInterstitialAdapter;Lcom/google/ads/AdRequest$ErrorCode;)V:GetOnFailedToReceiveAd_Lcom_google_ads_mediation_MediationInterstitialAdapter_Lcom_google_ads_AdRequest_ErrorCode_Handler:Google.Ads.Mediation.IMediationInterstitialListenerInvoker, GooglePlayServicesLib\n" +
			"n_onLeaveApplication:(Lcom/google/ads/mediation/MediationInterstitialAdapter;)V:GetOnLeaveApplication_Lcom_google_ads_mediation_MediationInterstitialAdapter_Handler:Google.Ads.Mediation.IMediationInterstitialListenerInvoker, GooglePlayServicesLib\n" +
			"n_onPresentScreen:(Lcom/google/ads/mediation/MediationInterstitialAdapter;)V:GetOnPresentScreen_Lcom_google_ads_mediation_MediationInterstitialAdapter_Handler:Google.Ads.Mediation.IMediationInterstitialListenerInvoker, GooglePlayServicesLib\n" +
			"n_onReceivedAd:(Lcom/google/ads/mediation/MediationInterstitialAdapter;)V:GetOnReceivedAd_Lcom_google_ads_mediation_MediationInterstitialAdapter_Handler:Google.Ads.Mediation.IMediationInterstitialListenerInvoker, GooglePlayServicesLib\n" +
			"";
		mono.android.Runtime.register ("Google.Ads.Mediation.IMediationInterstitialListenerImplementor, GooglePlayServicesLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", MediationInterstitialListenerImplementor.class, __md_methods);
	}


	public MediationInterstitialListenerImplementor () throws java.lang.Throwable
	{
		super ();
		if (getClass () == MediationInterstitialListenerImplementor.class)
			mono.android.TypeManager.Activate ("Google.Ads.Mediation.IMediationInterstitialListenerImplementor, GooglePlayServicesLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onDismissScreen (com.google.ads.mediation.MediationInterstitialAdapter p0)
	{
		n_onDismissScreen (p0);
	}

	private native void n_onDismissScreen (com.google.ads.mediation.MediationInterstitialAdapter p0);


	public void onFailedToReceiveAd (com.google.ads.mediation.MediationInterstitialAdapter p0, com.google.ads.AdRequest.ErrorCode p1)
	{
		n_onFailedToReceiveAd (p0, p1);
	}

	private native void n_onFailedToReceiveAd (com.google.ads.mediation.MediationInterstitialAdapter p0, com.google.ads.AdRequest.ErrorCode p1);


	public void onLeaveApplication (com.google.ads.mediation.MediationInterstitialAdapter p0)
	{
		n_onLeaveApplication (p0);
	}

	private native void n_onLeaveApplication (com.google.ads.mediation.MediationInterstitialAdapter p0);


	public void onPresentScreen (com.google.ads.mediation.MediationInterstitialAdapter p0)
	{
		n_onPresentScreen (p0);
	}

	private native void n_onPresentScreen (com.google.ads.mediation.MediationInterstitialAdapter p0);


	public void onReceivedAd (com.google.ads.mediation.MediationInterstitialAdapter p0)
	{
		n_onReceivedAd (p0);
	}

	private native void n_onReceivedAd (com.google.ads.mediation.MediationInterstitialAdapter p0);

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
