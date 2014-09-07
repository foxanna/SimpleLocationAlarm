package mono.com.google.android.gms.cast;


public class RemoteMediaPlayer_OnMetadataUpdatedListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.google.android.gms.cast.RemoteMediaPlayer.OnMetadataUpdatedListener
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onMetadataUpdated:()V:GetOnMetadataUpdatedHandler:Android.Gms.Cast.RemoteMediaPlayer/IOnMetadataUpdatedListenerInvoker, GooglePlayServicesLib\n" +
			"";
		mono.android.Runtime.register ("Android.Gms.Cast.RemoteMediaPlayer/IOnMetadataUpdatedListenerImplementor, GooglePlayServicesLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", RemoteMediaPlayer_OnMetadataUpdatedListenerImplementor.class, __md_methods);
	}


	public RemoteMediaPlayer_OnMetadataUpdatedListenerImplementor () throws java.lang.Throwable
	{
		super ();
		if (getClass () == RemoteMediaPlayer_OnMetadataUpdatedListenerImplementor.class)
			mono.android.TypeManager.Activate ("Android.Gms.Cast.RemoteMediaPlayer/IOnMetadataUpdatedListenerImplementor, GooglePlayServicesLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onMetadataUpdated ()
	{
		n_onMetadataUpdated ();
	}

	private native void n_onMetadataUpdated ();

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
