using System.Collections.Generic;

namespace JoystickVisualizer.Helper
{
    public class SupportedDeviceService
    {
        public Dictionary<string, string> Devices { get; }

        public SupportedDeviceService()
        {
            this.Devices = new Dictionary<string, string>()
            {
                { "044f:b10a", "Thrustmaster T.16000M" },
                { "044f:b687", "Thrustmaster T.16000M Throttle" },
                { "044f:b678", "Thrustmaster T.Flight Rudder Car Mode" },
                { "044f:b679", "Thrustmaster T.Flight Rudder" },
                { "044f:b108", "Thrustmaster T.Flight HOTAS X" },
                { "044f:0402", "Thrustmaster Warthog Joystick" },
                { "044f:0404", "Thrustmaster Warthog Throttle" },
                { "044f:ffff", "Thrustmaster Warthog Combined" },
                { "045e:02ff", "Controller (Xbox One For Windows)" },
                { "045e:001b", "SideWinder Force Feedback 2 Joystick" },
                { "06a3:0763", "Saitek Rudder Pedals" },
                { "06a3:0764", "Saitek Combat Rudder Pedals" },
                { "046d:c215", "Logitech 3D Pro" },
                { "06a3:075c", "Saitek X52 HOTAS" },
                { "06a3:0762", "Saitek X52 Pro HOTAS" }
            };
        }

        public bool IsSupported(string usbId)
        {
            return this.Devices.ContainsKey(usbId);
        }
    }
}
