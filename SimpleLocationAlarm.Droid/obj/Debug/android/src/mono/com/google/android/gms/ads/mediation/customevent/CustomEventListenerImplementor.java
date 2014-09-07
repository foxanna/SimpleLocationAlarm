package mono.com.google.android.gms.ads.mediation.customevent;


public class CustomEventListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.google.android.gms.ads.mediation.customevent.CustomEventListener
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onAdClicked:()V:GetOnAdClickedHandler:Android.Gms.Ads.Mediation.CustomEvent.ICustomEventListenerInvoker, GooglePlayServicesLib\n" +
			"n_onAdClosed:()V:GetOnAdClosedHandler:Android.Gms.Ads.Mediation.CustomEvent.ICustomEventListenerInvoker, GooglePlayServicesLib\n" +
			"n_onAdFailedToLoad:(I)V:GetOnAdFailedToLoad_IHandler:Android.Gms.Ads.Mediation.CustomEvent.ICustomEventListenerInvoker, GooglePlayServicesLib\n" +
			"n_onAdLeftApplication:()V:GetOnAdLeftApplicationHandler:Android.Gms.Ads.Mediation.CustomEvent.ICustomEventListenerInvoker, GooglePlayServicesLib\n" +
			"n_onAdOpened:()V:GetOnAdOpenedHandler:Android.Gms.Ads.Mediation.CustomEvent.ICustomEventListenerInvoker, GooglePlayServicesLib\n" +
			"";
		mono.android.Runtime.register ("Android.Gms.Ads.Mediation.CustomEvent.ICustomEventListenerImplementor, GooglePlayServicesLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", CustomEventListenerImplementor.class, __md_methods);
	}


	public CustomEventListenerImplementor () throws java.lang.Throwable
	{
		super ();
		if (getClass () == CustomEventListenerImplementor.class)
			mono.android.TypeManager.Activate ("Android.Gms.Ads.Mediation.CustomEvent.ICustomEventListenerImplementor, GooglePlayServicesLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onAdClicked ()
	{
		n_onAdClicked ();
	}

	private native void n_onAdClicked ();


	public void onAdClosed ()
	{
		n_onAdClosed ();
	}

	private native void n_onAdClosed ();


	public void onAdFailedToLoad (int p0)
	{
		n_onAdFailedToLoad (p0);
	}

	private native void n_onAdFailedToLoad (int p0);


	public void onAdLeftApplication ()
	{
		n_onAdLeftApplication ();
	}

	private native void n_onAdLeftApplication ();


	public void onAdOpened ()
	{
		n_onAdOpened ();
	}

	private native void n_onAdOpened ();

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
