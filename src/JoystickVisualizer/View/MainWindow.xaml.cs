using System.Linq;
using System.Windows;
using JoystickVisualizer.Service;
using JoystickVisualizer.ViewModel;
using Screen = System.Windows.Forms.Screen;

namespace JoystickVisualizer.View
{
    public partial class MainWindow : Window
    {
        private MainViewModel ViewModel => (MainViewModel)this.DataContext;

        private readonly string defaultView;
        private readonly string compactView;

        private double oldWidth;
        private double oldHeight;
        private double oldLeft;
        private double oldTop;

        public MainWindow()
        {
            this.InitializeComponent();

            this.defaultView = "DefaultView".GetLocalizedValue();
            this.compactView = "CompactView".GetLocalizedValue();

            this.ViewModel.Tracker.Track(this);
        }

        private void Switch_Click(object sender, RoutedEventArgs e)
        {
            if (this.SwitchButtonText.Text == this.compactView)
            {
                this.SwitchButtonText.Text = this.defaultView;

                this.oldWidth = this.Width;
                this.oldHeight = this.Height;

                this.Width = this.MinWidth;
                this.Height = this.MinHeight;

                this.WindowStyle = WindowStyle.None;
                this.ResizeMode = ResizeMode.NoResize;

                this.MoveWindowToBottomCorner();
            }
            else
            {
                this.SwitchButtonText.Text = this.compactView;

                this.Width = this.oldWidth;
                this.Height = this.oldHeight;

                this.WindowStyle = WindowStyle.SingleBorderWindow;
                this.ResizeMode = ResizeMode.CanResizeWithGrip;

                this.Left = this.oldLeft;
                this.Top = this.oldTop;
            }
        }

        private void MoveWindowToBottomCorner()
        {
            this.oldLeft = this.Left;
            this.oldTop = this.Top;

            var mainScreen = GetMainScreen();
            this.Left = mainScreen.Bounds.Width - this.Width;
            this.Top = mainScreen.Bounds.Height - this.Height;
        }

        private static Screen GetMainScreen()
        {
            var screen = (from item in Screen.AllScreens
                             where item.WorkingArea.Location.X == 0
                             select item).FirstOrDefault();

            return screen ?? Screen.AllScreens[0];
        }
    }
}
