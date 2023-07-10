using System.Collections.Generic;

namespace Enjlectric.ScriptableData
{
    /// <summary>
    /// Class for the Save Data as it gets serialized by the JSON Save Data Manager.
    /// </summary>
    [System.Serializable]
    public class SerializedSaveData
    {
        public List<string> identifiers = new List<string>();
        public List<string> values = new List<string>();
    }
}