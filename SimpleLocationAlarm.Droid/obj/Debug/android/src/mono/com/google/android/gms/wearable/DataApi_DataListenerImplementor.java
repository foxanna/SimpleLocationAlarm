package mono.com.google.android.gms.wearable;


public class DataApi_DataListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.google.android.gms.wearable.DataApi.DataListener
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onDataChanged:(Lcom/google/android/gms/wearable/DataEventBuffer;)V:GetOnDataChanged_Lcom_google_android_gms_wearable_DataEventBuffer_Handler:Android.Gms.Wearable.IDataApiDataListenerInvoker, GooglePlayServicesLib\n" +
			"";
		mono.android.Runtime.register ("Android.Gms.Wearable.IDataApiDataListenerImplementor, GooglePlayServicesLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", DataApi_DataListenerImplementor.class, __md_methods);
	}


	public DataApi_DataListenerImplementor () throws java.lang.Throwable
	{
		super ();
		if (getClass () == DataApi_DataListenerImplementor.class)
			mono.android.TypeManager.Activate ("Android.Gms.Wearable.IDataApiDataListenerImplementor, GooglePlayServicesLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onDataChanged (com.google.android.gms.wearable.DataEventBuffer p0)
	{
		n_onDataChanged (p0);
	}

	private native void n_onDataChanged (com.google.android.gms.wearable.DataEventBuffer p0);

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
