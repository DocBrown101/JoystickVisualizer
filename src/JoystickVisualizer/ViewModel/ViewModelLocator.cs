namespace JoystickVisualizer.ViewModel
{
    using System.Windows;
    using Autofac;
    using Jot;
    using JoystickVisualizer.Helper;

    public class ViewModelLocator
    {
        private readonly IContainer container;

        public ViewModelLocator()
        {
            var builder = new ContainerBuilder();

            builder.Register((context) =>
            {
                return GetTracker();
            }).SingleInstance();

            builder.RegisterType<AppSettings>().SingleInstance();
            builder.RegisterType<SupportedDeviceService>().SingleInstance();

            builder.RegisterType<MainViewModel>().SingleInstance();

            this.container = builder.Build();
        }

        public MainViewModel Main => this.container.Resolve<MainViewModel>();

        private static Tracker GetTracker()
        {
            var tracker = new Tracker();

            tracker.Configure<Window>()
                   .Id(w => w.Name, SystemParameters.VirtualScreenWidth)
                   .Properties(w => new { w.Top, w.Width, w.Height, w.Left, w.WindowState })
                   .PersistOn(nameof(Window.Closing))
                   .StopTrackingOn(nameof(Window.Closing));

            tracker.Configure<AppSettings>()
                   .Properties(p => new { p.IsGamePadMode, p.PollingRate })
                   .PersistOn("Exit", Application.Current)
                   .StopTrackingOn("Exit", Application.Current);

            return tracker;
        }
    }
}
