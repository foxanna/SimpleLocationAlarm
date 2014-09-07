package mono.com.google.android.gms.games.request;


public class OnRequestReceivedListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.google.android.gms.games.request.OnRequestReceivedListener
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onRequestReceived:(Lcom/google/android/gms/games/request/GameRequest;)V:GetOnRequestReceived_Lcom_google_android_gms_games_request_GameRequest_Handler:Android.Gms.Games.Request.IOnRequestReceivedListenerInvoker, GooglePlayServicesLib\n" +
			"n_onRequestRemoved:(Ljava/lang/String;)V:GetOnRequestRemoved_Ljava_lang_String_Handler:Android.Gms.Games.Request.IOnRequestReceivedListenerInvoker, GooglePlayServicesLib\n" +
			"";
		mono.android.Runtime.register ("Android.Gms.Games.Request.IOnRequestReceivedListenerImplementor, GooglePlayServicesLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", OnRequestReceivedListenerImplementor.class, __md_methods);
	}


	public OnRequestReceivedListenerImplementor () throws java.lang.Throwable
	{
		super ();
		if (getClass () == OnRequestReceivedListenerImplementor.class)
			mono.android.TypeManager.Activate ("Android.Gms.Games.Request.IOnRequestReceivedListenerImplementor, GooglePlayServicesLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onRequestReceived (com.google.android.gms.games.request.GameRequest p0)
	{
		n_onRequestReceived (p0);
	}

	private native void n_onRequestReceived (com.google.android.gms.games.request.GameRequest p0);


	public void onRequestRemoved (java.lang.String p0)
	{
		n_onRequestRemoved (p0);
	}

	private native void n_onRequestRemoved (java.lang.String p0);

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
