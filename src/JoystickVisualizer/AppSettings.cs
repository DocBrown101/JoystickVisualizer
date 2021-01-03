using System;

namespace JoystickVisualizer
{
    [Serializable]
    public class AppSettings
    {
        public bool IsGamePadMode { get; set; }

        public int PollingRate { get; set; }
    }
}
