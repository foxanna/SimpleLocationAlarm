package mono.com.google.ads.mediation.customevent;


public class CustomEventListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.google.ads.mediation.customevent.CustomEventListener
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onDismissScreen:()V:GetOnDismissScreenHandler:Google.Ads.Mediation.CustomEvent.ICustomEventListenerInvoker, GooglePlayServicesLib\n" +
			"n_onFailedToReceiveAd:()V:GetOnFailedToReceiveAdHandler:Google.Ads.Mediation.CustomEvent.ICustomEventListenerInvoker, GooglePlayServicesLib\n" +
			"n_onLeaveApplication:()V:GetOnLeaveApplicationHandler:Google.Ads.Mediation.CustomEvent.ICustomEventListenerInvoker, GooglePlayServicesLib\n" +
			"n_onPresentScreen:()V:GetOnPresentScreenHandler:Google.Ads.Mediation.CustomEvent.ICustomEventListenerInvoker, GooglePlayServicesLib\n" +
			"";
		mono.android.Runtime.register ("Google.Ads.Mediation.CustomEvent.ICustomEventListenerImplementor, GooglePlayServicesLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", CustomEventListenerImplementor.class, __md_methods);
	}


	public CustomEventListenerImplementor () throws java.lang.Throwable
	{
		super ();
		if (getClass () == CustomEventListenerImplementor.class)
			mono.android.TypeManager.Activate ("Google.Ads.Mediation.CustomEvent.ICustomEventListenerImplementor, GooglePlayServicesLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onDismissScreen ()
	{
		n_onDismissScreen ();
	}

	private native void n_onDismissScreen ();


	public void onFailedToReceiveAd ()
	{
		n_onFailedToReceiveAd ();
	}

	private native void n_onFailedToReceiveAd ();


	public void onLeaveApplication ()
	{
		n_onLeaveApplication ();
	}

	private native void n_onLeaveApplication ();


	public void onPresentScreen ()
	{
		n_onPresentScreen ();
	}

	private native void n_onPresentScreen ();

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
