using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace JoystickVisualizer.Model
{
    public class JoystickModel
    {
        public string UsbId { get; }

        public bool IsSupported { get; }

        public DeviceInstance DeviceInstance { get; }

        public string Name => this.DeviceInstance.InstanceName;

        public string Guid => this.DeviceInstance.InstanceGuid.ToString();

        public SortedDictionary<string, JoystickUpdate> CurrentState { get; } = new SortedDictionary<string, JoystickUpdate>();
        
        private readonly Joystick joystick;
        private bool NotPollable = false;

        public JoystickModel(DirectInput directInput, DeviceInstance deviceInstance, bool isSupported)
        {
            this.IsSupported = isSupported;
            this.DeviceInstance = deviceInstance;
            this.UsbId = ProductGuidToUSBID(this.DeviceInstance.ProductGuid);
            
            this.joystick = new Joystick(directInput, deviceInstance.InstanceGuid);
            this.joystick.Properties.BufferSize = 32;
        }

        public static string ProductGuidToUSBID(Guid guid)
        {
            return Regex.Replace(guid.ToString(), @"(^....)(....).*$", "$2:$1");
        }

        public List<JoystickInputModel> GetJoystickInputs()
        {
            var updatedStates = new List<JoystickInputModel>();

            if (this.NotPollable)
            {
                return updatedStates;
            }

            try
            {
                this.joystick.Poll();
            }
            catch (Exception)
            {
                this.NotPollable = true;
                return updatedStates;
            }

            foreach (var joystickUpdate in this.joystick.GetBufferedData())
            {
                this.CurrentState[joystickUpdate.Offset.ToString()] = joystickUpdate;
                updatedStates.Add(new JoystickInputModel(joystickUpdate));
            }

            return updatedStates;
        }

        internal void Unacquire()
        {
            try
            {
                this.joystick.Unacquire();
            }
            catch (Exception) { }
        }

        internal void Acquire()
        {
            this.joystick.Acquire();
        }
    }
}
