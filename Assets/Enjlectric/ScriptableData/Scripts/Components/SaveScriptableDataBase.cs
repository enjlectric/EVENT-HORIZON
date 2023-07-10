using System.Collections.Generic;
using UnityEngine;

namespace Enjlectric.ScriptableData
{
    /// <summary>
    /// Base class for implementations of a Save Game manager.
    /// </summary>
    public abstract class SaveScriptableDataBase : MonoBehaviour
    {
        [SerializeField] protected SaveDataGroup _saveGroup;

        protected Dictionary<string, ScriptableDataBase> _scriptableDataMap = new Dictionary<string, ScriptableDataBase>();

        protected virtual void Awake()
        {
            if (_scriptableDataMap.Count == 0)
            {
                BuildScriptableDataMap();
            }
        }

        /// <summary>
        /// Builds a name-value-pair of all registered save data, so that save values can be saved by identifier.
        /// </summary>
        protected virtual void BuildScriptableDataMap()
        {
            foreach (var value in _saveGroup.SaveDataValues)
            {
                _scriptableDataMap.Add(value.GetIdentifier(), value);
            }
        }

        /// <summary>
        /// Restore this SaveData from a specific save slot.
        /// </summary>
        /// <param name="saveSlot">The index of the save slot to restore from.</param>

        public abstract void Restore(int saveSlot);

        /// <summary>
        /// Save this SaveData to a specific save slot.
        /// </summary>
        /// <param name="saveSlot">The index of the save slot to save to.</param>

        public abstract void Save(int saveSlot);

        /// <summary>
        /// Reset all saved data to their default values.
        /// </summary>
        public abstract void ResetToDefault();

        /// <summary>
        /// Check whether or not a save game exists in the given slot.
        /// </summary>
        /// <param name="saveSlot">The index of the save slot to check.</param>
        public abstract bool Exists(int saveSlot);

        /// <summary>
        /// Clear (delete) a specific save slot.
        /// </summary>
        /// <param name="saveSlot">The index of the save slot to clear.</param>
        public abstract void Clear(int saveSlot);
    }
}