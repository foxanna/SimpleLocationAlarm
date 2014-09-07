package mono.com.google.android.gms.wearable;


public class MessageApi_MessageListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.google.android.gms.wearable.MessageApi.MessageListener
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onMessageReceived:(Lcom/google/android/gms/wearable/MessageEvent;)V:GetOnMessageReceived_Lcom_google_android_gms_wearable_MessageEvent_Handler:Android.Gms.Wearable.IMessageApiMessageListenerInvoker, GooglePlayServicesLib\n" +
			"";
		mono.android.Runtime.register ("Android.Gms.Wearable.IMessageApiMessageListenerImplementor, GooglePlayServicesLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", MessageApi_MessageListenerImplementor.class, __md_methods);
	}


	public MessageApi_MessageListenerImplementor () throws java.lang.Throwable
	{
		super ();
		if (getClass () == MessageApi_MessageListenerImplementor.class)
			mono.android.TypeManager.Activate ("Android.Gms.Wearable.IMessageApiMessageListenerImplementor, GooglePlayServicesLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onMessageReceived (com.google.android.gms.wearable.MessageEvent p0)
	{
		n_onMessageReceived (p0);
	}

	private native void n_onMessageReceived (com.google.android.gms.wearable.MessageEvent p0);

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
