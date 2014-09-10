using System;
using Android.Widget;
using Android.Views;
using Android.Content;
using Android.App;
using System.Threading.Tasks;
using System.Threading;
using Android.Util;

namespace SimpleLocationAlarm.Droid
{
	public class UndoBar
	{
		public event EventHandler Undo;

		public event EventHandler Discard;

		readonly PopupWindow _popup;

		readonly View _parentView;

		bool _undone;

		CancellationTokenSource _cancellationTokenSource;

		public UndoBar (Context context, string text, View parentView)
		{
			_parentView = parentView;

			var view = (context.GetSystemService (Context.LayoutInflaterService) as LayoutInflater).Inflate (Resource.Layout.undo_bar, null);

			var undoButton = view.FindViewById (Resource.Id.undo_button);
			undoButton.Click += OnUndoClick;

			var titleTextView = view.FindViewById<TextView> (Resource.Id.undo_message);
			titleTextView.Text = text;

			_popup = new PopupWindow (view, ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent, false);
			_popup.AnimationStyle = Resource.Style.popup_fade_animation;            
		}

		void Popup_DismissEvent (object sender, EventArgs e)
		{
			OnDiscard ();
		}

		public void Show ()
		{
			_popup.Width = (int) Math.Min (Application.Context.Resources.DisplayMetrics.Density * 400, _parentView.Width * 0.9f);
			_popup.DismissEvent += Popup_DismissEvent;
			_popup.ShowAtLocation (_parentView, GravityFlags.CenterHorizontal | GravityFlags.Bottom, 0, 60);

			SchedulePopupClose ();
		}

		void OnUndoClick (object sender, EventArgs e)
		{
			_undone = true;

			Hide ();
		}

		void OnDiscard ()
		{
			_popup.DismissEvent -= Popup_DismissEvent;
			_cancellationTokenSource.Cancel ();

			var handler = _undone ? Undo : Discard;          

			if (handler != null)
				handler (this, EventArgs.Empty);            
		}

		async void SchedulePopupClose ()
		{
			_cancellationTokenSource = new CancellationTokenSource ();

			try {
				var ui = TaskScheduler.FromCurrentSynchronizationContext ();
				await Task.Delay (5000, _cancellationTokenSource.Token);
				await Task.Factory.StartNew (OnPopupTimeOut, _cancellationTokenSource.Token, TaskCreationOptions.None, ui);
			} catch (OperationCanceledException) {
				Log.Debug ("Undobar", "OperationCanceledException");
			}
		}

		void OnPopupTimeOut ()
		{
			if (_cancellationTokenSource.IsCancellationRequested || _undone)
				return;

			Hide ();
		}

		public void Hide ()
		{
			if (_popup.IsShowing) {
				_popup.Dismiss ();
			}
		}
	}
}