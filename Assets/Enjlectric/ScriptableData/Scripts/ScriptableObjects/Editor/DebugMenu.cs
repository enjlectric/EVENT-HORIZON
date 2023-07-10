using UnityEditor;

namespace Enjlectric.ScriptableData.Editor
{
    /// <summary>
    /// Creates the Menu item for toggling use of production values.
    /// </summary>
    public static class DebugMenu
    {
        private const string MenuName = "Tools/ScriptableData/Use Production Values";
        private const string SettingName = "ScriptableDataUseProductionValues";

        public static bool IsEnabled
        {
            get { return EditorPrefs.GetBool(SettingName, false); }
            set { EditorPrefs.SetBool(SettingName, value); }
        }

        [MenuItem(MenuName)]
        private static void ToggleAction()
        {
            IsEnabled = !IsEnabled;
        }

        [MenuItem(MenuName, true)]
        private static bool ToggleActionValidate()
        {
            Menu.SetChecked(MenuName, IsEnabled);
            return true;
        }
    }
}