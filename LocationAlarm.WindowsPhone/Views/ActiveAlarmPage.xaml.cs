using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using LocationAlarm.PCL;
using LocationAlarm.PCL.ViewModels;
using LocationAlarm.WindowsPhone.Common;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace LocationAlarm.WindowsPhone.Views
{
	public sealed partial class ActiveAlarmPage : Page
	{
		public NavigationHelper NavigationHelper { get; private set; }

		public ActiveAlarmViewModel ViewModel { get; private set; }

		public ActiveAlarmPage()
		{
			ViewModel = IoC.Get<ActiveAlarmViewModel>();

			this.InitializeComponent();

			NavigationHelper = new NavigationHelper(this);
			NavigationHelper.LoadState += this.NavigationHelper_LoadState;
			NavigationHelper.SaveState += this.NavigationHelper_SaveState;
		}

		#region NavigationHelper registration

		/// <summary>
		/// The methods provided in this section are simply used to allow
		/// NavigationHelper to respond to the page's navigation methods.
		/// <para>
		/// Page specific logic should be placed in event handlers for the
		/// <see cref="NavigationHelper.LoadState"/>
		/// and <see cref="NavigationHelper.SaveState"/>.
		/// The navigation parameter is available in the LoadState method
		/// in addition to page state preserved during an earlier session.
		/// </para>
		/// </summary>
		/// <param name="e">Event data that describes how this page was reached.</param>
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			NavigationHelper.OnNavigatedTo(e);
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			NavigationHelper.OnNavigatedFrom(e);
		}

		#endregion

		private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
		{
		}

		private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
		{
			ViewModel.ActiveAlarm = e.NavigationParameter as AlarmItemViewModel;
		}
	}
}
