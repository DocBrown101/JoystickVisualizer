namespace JoystickVisualizer.ViewModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows.Input;
    using System.Windows.Threading;
    using Jot;
    using JoystickVisualizer.Helper;
    using JoystickVisualizer.Model;
    using JoystickVisualizer.Service;
    using Microsoft.Xna.Framework.Input;
    using SharpDX.DirectInput;

    public class MainViewModel : Observable
    {
        private readonly SupportedDeviceService supportedDeviceService;
        private readonly AppSettings appSettings;
        private readonly DirectInput directInput;
        private readonly DispatcherTimer readInputTimer;
        private readonly DispatcherTimer refreshDevicesTimer;

        private JoystickModel selectedJoystick;
        private bool isGamePadMode;
        private double x;
        private double y;
        private double acceleration;

        public MainViewModel()
        { }

        public MainViewModel(AppSettings appSettings, Tracker tracker, SupportedDeviceService supportedDeviceService)
        {
            this.Tracker = tracker;
            this.Tracker.Track(appSettings);

            this.appSettings = appSettings;
            this.supportedDeviceService = supportedDeviceService;

            this.directInput = new DirectInput();
            this.readInputTimer = new DispatcherTimer(DispatcherPriority.Normal);
            this.refreshDevicesTimer = new DispatcherTimer(DispatcherPriority.Background);

            this.Joysticks = new ObservableCollection<JoystickModel>();
            this.JoystickInputs = new ObservableCollection<JoystickInputModel>();

            this.LoadedCommand = new RelayCommand(this.Loaded);

            this.IsGamePadMode = this.appSettings.IsGamePadMode;

            this.InitTimers();
        }

        public ObservableCollection<JoystickModel> Joysticks { get; }

        public ObservableCollection<JoystickInputModel> JoystickInputs { get; }

        public JoystickModel SelectedJoystick
        {
            get => this.selectedJoystick;
            set
            {
                this.selectedJoystick?.Unacquire();

                this.Set(ref this.selectedJoystick, value);

                this.selectedJoystick?.Acquire();

                this.ClearJoystickInputs();
            }
        }

        public bool IsGamePadMode
        {
            get => this.isGamePadMode;
            set
            {
                this.Set(ref this.isGamePadMode, value);
                this.ClearJoystickInputs();
                this.appSettings.IsGamePadMode = value;
            }
        }

        public double X
        {
            get => this.x;
            set => this.Set(ref this.x, value);
        }

        public double Y
        {
            get => this.y;
            set => this.Set(ref this.y, value);
        }

        public double Acceleration
        {
            get => this.acceleration;
            set => this.Set(ref this.acceleration, value);
        }

        public ICommand LoadedCommand { get; }

        public Tracker Tracker { get; }

        public static string Title => WindowTitleService.GetMainWindowTitle();

        private void InitTimers()
        {
            this.readInputTimer.Interval = TimeSpan.FromMilliseconds(34);
            this.readInputTimer.Tick += (sender, e) => { this.ReadInputTimerTick(); };
            this.readInputTimer.Start();

            this.refreshDevicesTimer.Interval = TimeSpan.FromSeconds(2);
            this.refreshDevicesTimer.Tick += (sender, e) => { this.ScanJoysticks(); };
            this.refreshDevicesTimer.Start();
        }

        private void Loaded()
        {
            this.ScanJoysticks();
        }

        private void ScanJoysticks()
        {
            var allDevices = this.directInput.GetDevices();

            var addedDevices = allDevices.Where(p => this.Joysticks.All(j => j.DeviceInstance.InstanceGuid != p.InstanceGuid)).OrderBy(d => d.ProductName).ToList();
            var removedDevices = this.Joysticks.Where(p => allDevices.All(j => j.InstanceGuid != p.DeviceInstance.InstanceGuid)).ToList();

            foreach (var deviceInstance in addedDevices)
            {
                var usbid = JoystickModel.ProductGuidToUSBID(deviceInstance.ProductGuid);
                var isSupported = this.supportedDeviceService.IsSupported(usbid);
                var joystickModel = new JoystickModel(this.directInput, deviceInstance, isSupported);

                if (isSupported)
                {
                    this.Joysticks.Insert(0, joystickModel);
                    this.SelectedJoystick ??= joystickModel;
                }
                else
                {
                    this.Joysticks.Add(joystickModel);
                }
            }

            foreach (var deviceInstance in removedDevices)
            {
                deviceInstance.Unacquire();
                this.Joysticks.Remove(deviceInstance);
            }
        }

        private void ReadInputTimerTick()
        {
            try
            {
                if (this.isGamePadMode)
                {
                    var gamePad = GamePad.GetState(0);
                    this.X = gamePad.ThumbSticks.Left.X.ConvertToPercentValue(-1, 1);
                    this.Y = gamePad.ThumbSticks.Left.Y.ConvertToPercentValue(-1, 1);
                    return;
                }

                if (this.SelectedJoystick == null)
                {
                    return;
                }

                var inputs = this.SelectedJoystick.GetJoystickInputs();
                if (inputs.Count == 0)
                {
                    return;
                }

                this.JoystickInputs.Clear();
                foreach (var inputState in this.SelectedJoystick.CurrentState.Values)
                {
                    switch (inputState.Offset)
                    {
                        case JoystickOffset.X:
                            this.X = inputState.Value.ConvertToPercentValue(0, 65535);
                            break;
                        case JoystickOffset.Y:
                            this.Y = inputState.Value.ConvertToPercentValue(0, 65535);
                            break;
                        case JoystickOffset.Sliders0:
                            this.Acceleration = inputState.Value;
                            break;

                        default:
                            this.JoystickInputs.Add(new JoystickInputModel(inputState));
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failure when running device Update()");
                Debug.WriteLine(ex.Message);
            }
        }

        private void ClearJoystickInputs()
        {
            this.X = 0;
            this.Y = 0;
            this.Acceleration = 0;
            this.JoystickInputs.Clear();
        }
    }
}
