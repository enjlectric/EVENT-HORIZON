using System;

namespace Enjlectric.ScriptableData
{
    /// <summary>
    /// Resets the attached field to the value of one of two other fields based on the UseProductionValues editor pref.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class AutoResetAttribute : Attribute
    {
        public string copySourceName;
        public string copySourceTestName;

        public AutoResetAttribute(string copySourceName, string copySourceTestName)
        {
            this.copySourceName = copySourceName;
            this.copySourceTestName = copySourceTestName;
        }
    }
}