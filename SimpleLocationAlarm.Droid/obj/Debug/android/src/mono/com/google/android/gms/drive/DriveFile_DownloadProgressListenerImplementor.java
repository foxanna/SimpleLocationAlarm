package mono.com.google.android.gms.drive;


public class DriveFile_DownloadProgressListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.google.android.gms.drive.DriveFile.DownloadProgressListener
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onProgress:(JJ)V:GetOnProgress_JJHandler:Android.Gms.Drive.IDriveFileDownloadProgressListenerInvoker, GooglePlayServicesLib\n" +
			"";
		mono.android.Runtime.register ("Android.Gms.Drive.IDriveFileDownloadProgressListenerImplementor, GooglePlayServicesLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", DriveFile_DownloadProgressListenerImplementor.class, __md_methods);
	}


	public DriveFile_DownloadProgressListenerImplementor () throws java.lang.Throwable
	{
		super ();
		if (getClass () == DriveFile_DownloadProgressListenerImplementor.class)
			mono.android.TypeManager.Activate ("Android.Gms.Drive.IDriveFileDownloadProgressListenerImplementor, GooglePlayServicesLib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onProgress (long p0, long p1)
	{
		n_onProgress (p0, p1);
	}

	private native void n_onProgress (long p0, long p1);

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
