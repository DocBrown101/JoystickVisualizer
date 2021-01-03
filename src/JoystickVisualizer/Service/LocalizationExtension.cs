using WPFLocalizeExtension.Engine;

namespace JoystickVisualizer.Service
{
    public static class LocalizationExtension
    {
        public static string GetLocalizedValue(this string self)
        {
            return LocalizeDictionary.Instance.GetLocalizedObject(self, null, LocalizeDictionary.Instance.Culture).ToString();
        }
    }
}
