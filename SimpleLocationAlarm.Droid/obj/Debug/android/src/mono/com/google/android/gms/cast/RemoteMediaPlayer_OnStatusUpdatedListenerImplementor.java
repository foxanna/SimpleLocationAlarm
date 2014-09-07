package mono.com.google.android.gms.cast;


public class RemoteMediaPlayer_OnStatusUpdatedListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.google.android.gms.cast.RemoteMediaPlayer.OnStatusUpdatedListener
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onStatusUpdated:()V:GetOnStatusUpdatedHandler:Android.Gms.Cast.RemoteMediaPlayer/IOnStatusUpdatedListenerInvoker, GooglePlayServicesLib\n" +
			"";
		mono.android.Runtime.register ("Android.Gms.Cast.RemoteMediaPlayer/IOnStatusUpdatedListenerImplementor, GooglePlayServicesLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", RemoteMediaPlayer_OnStatusUpdatedListenerImplementor.class, __md_methods);
	}


	public RemoteMediaPlayer_OnStatusUpdatedListenerImplementor () throws java.lang.Throwable
	{
		super ();
		if (getClass () == RemoteMediaPlayer_OnStatusUpdatedListenerImplementor.class)
			mono.android.TypeManager.Activate ("Android.Gms.Cast.RemoteMediaPlayer/IOnStatusUpdatedListenerImplementor, GooglePlayServicesLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onStatusUpdated ()
	{
		n_onStatusUpdated ();
	}

	private native void n_onStatusUpdated ();

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
