package mono.com.google.android.gms.wearable;


public class NodeApi_NodeListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.google.android.gms.wearable.NodeApi.NodeListener
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onPeerConnected:(Lcom/google/android/gms/wearable/Node;)V:GetOnPeerConnected_Lcom_google_android_gms_wearable_Node_Handler:Android.Gms.Wearable.INodeApiNodeListenerInvoker, GooglePlayServicesLib\n" +
			"n_onPeerDisconnected:(Lcom/google/android/gms/wearable/Node;)V:GetOnPeerDisconnected_Lcom_google_android_gms_wearable_Node_Handler:Android.Gms.Wearable.INodeApiNodeListenerInvoker, GooglePlayServicesLib\n" +
			"";
		mono.android.Runtime.register ("Android.Gms.Wearable.INodeApiNodeListenerImplementor, GooglePlayServicesLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", NodeApi_NodeListenerImplementor.class, __md_methods);
	}


	public NodeApi_NodeListenerImplementor () throws java.lang.Throwable
	{
		super ();
		if (getClass () == NodeApi_NodeListenerImplementor.class)
			mono.android.TypeManager.Activate ("Android.Gms.Wearable.INodeApiNodeListenerImplementor, GooglePlayServicesLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onPeerConnected (com.google.android.gms.wearable.Node p0)
	{
		n_onPeerConnected (p0);
	}

	private native void n_onPeerConnected (com.google.android.gms.wearable.Node p0);


	public void onPeerDisconnected (com.google.android.gms.wearable.Node p0)
	{
		n_onPeerDisconnected (p0);
	}

	private native void n_onPeerDisconnected (com.google.android.gms.wearable.Node p0);

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
