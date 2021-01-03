namespace JoystickVisualizer.Service
{
    public static class CalculationExtension
    {
        public static float ConvertToPercentValue(this float self, float min, float max)
        {
            return (self - min) * 100 / (max - min);
        }

        public static double ConvertToPercentValue(this int self, int min, int max)
        {
            return (self - min) * 100 / (max - min);
        }
    }
}
