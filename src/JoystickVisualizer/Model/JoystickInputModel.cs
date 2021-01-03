using SharpDX.DirectInput;

namespace JoystickVisualizer.Model
{
    public class JoystickInputModel
    {
        public JoystickUpdate JoystickUpdate { get; }

        public string Name { get; }

        public int Value { get; }

        public JoystickInputModel(JoystickUpdate joystickUpdate)
        {
            this.JoystickUpdate = joystickUpdate;
            this.Name = joystickUpdate.Offset.ToString();
            this.Value = joystickUpdate.Value;
        }

        public override string ToString()
        {
            return this.Name + "=" + this.Value;
        }
    }
}
