package mono.com.google.android.gms.drive.events;


public class DriveEvent_ListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.google.android.gms.drive.events.DriveEvent.Listener
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onEvent:(Lcom/google/android/gms/drive/events/DriveEvent;)V:GetOnEvent_Lcom_google_android_gms_drive_events_DriveEvent_Handler:Android.Gms.Drive.Events.IDriveEventListenerInvoker, GooglePlayServicesLib\n" +
			"";
		mono.android.Runtime.register ("Android.Gms.Drive.Events.IDriveEventListenerImplementor, GooglePlayServicesLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", DriveEvent_ListenerImplementor.class, __md_methods);
	}


	public DriveEvent_ListenerImplementor () throws java.lang.Throwable
	{
		super ();
		if (getClass () == DriveEvent_ListenerImplementor.class)
			mono.android.TypeManager.Activate ("Android.Gms.Drive.Events.IDriveEventListenerImplementor, GooglePlayServicesLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onEvent (com.google.android.gms.drive.events.DriveEvent p0)
	{
		n_onEvent (p0);
	}

	private native void n_onEvent (com.google.android.gms.drive.events.DriveEvent p0);

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
