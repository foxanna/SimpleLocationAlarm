package mono.com.google.ads.mediation;


public class MediationBannerListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.google.ads.mediation.MediationBannerListener
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onClick:(Lcom/google/ads/mediation/MediationBannerAdapter;)V:GetOnClick_Lcom_google_ads_mediation_MediationBannerAdapter_Handler:Google.Ads.Mediation.IMediationBannerListenerInvoker, GooglePlayServicesLib\n" +
			"n_onDismissScreen:(Lcom/google/ads/mediation/MediationBannerAdapter;)V:GetOnDismissScreen_Lcom_google_ads_mediation_MediationBannerAdapter_Handler:Google.Ads.Mediation.IMediationBannerListenerInvoker, GooglePlayServicesLib\n" +
			"n_onFailedToReceiveAd:(Lcom/google/ads/mediation/MediationBannerAdapter;Lcom/google/ads/AdRequest$ErrorCode;)V:GetOnFailedToReceiveAd_Lcom_google_ads_mediation_MediationBannerAdapter_Lcom_google_ads_AdRequest_ErrorCode_Handler:Google.Ads.Mediation.IMediationBannerListenerInvoker, GooglePlayServicesLib\n" +
			"n_onLeaveApplication:(Lcom/google/ads/mediation/MediationBannerAdapter;)V:GetOnLeaveApplication_Lcom_google_ads_mediation_MediationBannerAdapter_Handler:Google.Ads.Mediation.IMediationBannerListenerInvoker, GooglePlayServicesLib\n" +
			"n_onPresentScreen:(Lcom/google/ads/mediation/MediationBannerAdapter;)V:GetOnPresentScreen_Lcom_google_ads_mediation_MediationBannerAdapter_Handler:Google.Ads.Mediation.IMediationBannerListenerInvoker, GooglePlayServicesLib\n" +
			"n_onReceivedAd:(Lcom/google/ads/mediation/MediationBannerAdapter;)V:GetOnReceivedAd_Lcom_google_ads_mediation_MediationBannerAdapter_Handler:Google.Ads.Mediation.IMediationBannerListenerInvoker, GooglePlayServicesLib\n" +
			"";
		mono.android.Runtime.register ("Google.Ads.Mediation.IMediationBannerListenerImplementor, GooglePlayServicesLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", MediationBannerListenerImplementor.class, __md_methods);
	}


	public MediationBannerListenerImplementor () throws java.lang.Throwable
	{
		super ();
		if (getClass () == MediationBannerListenerImplementor.class)
			mono.android.TypeManager.Activate ("Google.Ads.Mediation.IMediationBannerListenerImplementor, GooglePlayServicesLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onClick (com.google.ads.mediation.MediationBannerAdapter p0)
	{
		n_onClick (p0);
	}

	private native void n_onClick (com.google.ads.mediation.MediationBannerAdapter p0);


	public void onDismissScreen (com.google.ads.mediation.MediationBannerAdapter p0)
	{
		n_onDismissScreen (p0);
	}

	private native void n_onDismissScreen (com.google.ads.mediation.MediationBannerAdapter p0);


	public void onFailedToReceiveAd (com.google.ads.mediation.MediationBannerAdapter p0, com.google.ads.AdRequest.ErrorCode p1)
	{
		n_onFailedToReceiveAd (p0, p1);
	}

	private native void n_onFailedToReceiveAd (com.google.ads.mediation.MediationBannerAdapter p0, com.google.ads.AdRequest.ErrorCode p1);


	public void onLeaveApplication (com.google.ads.mediation.MediationBannerAdapter p0)
	{
		n_onLeaveApplication (p0);
	}

	private native void n_onLeaveApplication (com.google.ads.mediation.MediationBannerAdapter p0);


	public void onPresentScreen (com.google.ads.mediation.MediationBannerAdapter p0)
	{
		n_onPresentScreen (p0);
	}

	private native void n_onPresentScreen (com.google.ads.mediation.MediationBannerAdapter p0);


	public void onReceivedAd (com.google.ads.mediation.MediationBannerAdapter p0)
	{
		n_onReceivedAd (p0);
	}

	private native void n_onReceivedAd (com.google.ads.mediation.MediationBannerAdapter p0);

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
