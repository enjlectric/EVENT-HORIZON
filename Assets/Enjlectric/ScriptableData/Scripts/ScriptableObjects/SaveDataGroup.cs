using System.Collections.Generic;
using UnityEngine;

namespace Enjlectric.ScriptableData
{
    [CreateAssetMenu(menuName = "Enjlectric/ScriptableData/SaveDataManager")]
    public class SaveDataGroup : ScriptableObject
    {
        [Tooltip("Filename of the files saved from this group. Any # characters will be replaced by the save slot.")]
        public string FileName = "save-#.save";

        [Tooltip("List of all SaveData ScriptableData objects to save to the file.")]
        [SerializeField]
        public List<ScriptableDataBase> SaveDataValues = new List<ScriptableDataBase>();
    }
}