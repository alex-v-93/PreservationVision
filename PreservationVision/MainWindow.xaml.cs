using System.Windows;
using System.Windows.Forms;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace PreservationVision
{
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly NotifyIcon _notifyIcon = new NotifyIcon();

        public IPreservationVisionModel Model { get; }

        private Parameters Params { get; set; }

        private void ShowAction(bool isShow)
        {
            if(isShow)
                Show();
            else
                Hide();
        }

        public MainWindow()
        {
            Model = new PreservationVisionModel(Dispatcher, ShowAction);
            InitializeComponent();
            _notifyIcon.Icon = Properties.Resources.eye;
            _notifyIcon.Visible = true;
            _notifyIcon.MouseDoubleClick += NotifyIconOnMouseDoubleClick;
            _notifyIcon.MouseClick += NotifyIconOnMouseClick;
            Params = new Parameters(Model);
        }

        private void NotifyIconOnMouseClick(object sender, MouseEventArgs mouseEventArgs)
        {
            if(mouseEventArgs.Button == MouseButtons.Right)
                System.Windows.Application.Current.Shutdown();
        }

        private void NotifyIconOnMouseDoubleClick(object sender, MouseEventArgs mouseEventArgs)
        {
            if (Params != null)
                Params.Visibility = Params.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
                
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Model.IsVisibleWindow = false;
        }
    }
}
