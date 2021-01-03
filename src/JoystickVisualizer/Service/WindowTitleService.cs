using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoystickVisualizer.Service
{
    public class WindowTitleService
    {
        public static string GetMainWindowTitle()
        {
            return IsDebugSession() == true ? "Joystick-Visualizer | DEBUG" : $"Joystick-Visualizer | {System.Windows.Forms.Application.ProductVersion}";
        }

        private static bool IsDebugSession()
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }
    }
}
