using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LocationAlarm.WindowsPhone.Views
{
    public sealed partial class PageTitleBar : UserControl
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title),
            typeof (string), typeof (PageTitleBar), new PropertyMetadata(null, TitleChanged));

        public PageTitleBar()
        {
            InitializeComponent();
        }

        public string Title
        {
            get { return (string) GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        private static void TitleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ((PageTitleBar) sender).TitleLabel.Text = (string) args.NewValue;
        }
    }
}