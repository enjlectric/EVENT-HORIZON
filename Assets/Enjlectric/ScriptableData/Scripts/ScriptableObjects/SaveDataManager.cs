using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace Enjlectric.ScriptableData
{
    [System.Serializable]
    public class SaveData
    {
        public List<string> identifiers = new List<string>();
        public List<string> values = new List<string>();
    }

    [CreateAssetMenu(menuName = "Enjlectric/ScriptableData/SaveDataManager")]
    public class SaveDataManager : ScriptableObject
    {
        public List<ScriptableDataBase> SaveDataValues = new List<ScriptableDataBase>();

        private Dictionary<string, ScriptableDataBase> ScriptableDataMap = new Dictionary<string, ScriptableDataBase>();
        void Awake()
        {
            if (ScriptableDataMap.Count == 0)
            {
                foreach (var value in SaveDataValues)
                {
                    ScriptableDataMap.Add(value.name, value);
                }
            }
        }

        public void Restore(int saveSlot = 1)
        {
            try
            {
                if (ScriptableDataMap.Count == 0)
                {
                    Awake();
                }
                var file = System.IO.File.ReadAllText(Application.persistentDataPath + "/save-" + saveSlot + ".save");
                if (file != null)
                {
                    SaveData sd = JsonConvert.DeserializeObject<SaveData>(file);
                    int i = 0;
                    foreach (var value in sd.identifiers)
                    {
                        if (ScriptableDataMap.ContainsKey(value))
                        {
                            ScriptableDataMap[value].Restore(sd.values[i]);
                        }
                        i++;
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.Log("No valid SaveGame found.");
            }
        }

        public void Save(int saveSlot = 1)
        {
            SaveData sd = new SaveData();
            foreach (var value in SaveDataValues)
            {
                var obj = value.Save();
                if (obj != null)
                {
                    sd.identifiers.Add(value.name);
                    sd.values.Add(JsonConvert.SerializeObject(obj));
                }
            }

            System.IO.FileStream file = System.IO.File.Create(Application.persistentDataPath + "/save-" + saveSlot + ".save");
            System.IO.StreamWriter writer = new System.IO.StreamWriter(file);
            var json = JsonConvert.SerializeObject(sd);
            writer.Write(json);
            writer.Flush();
            file.Close();
        }

        public void Reset()
        {
            foreach (var value in SaveDataValues)
            {
                value.ResetToDefault();
            }
        }

        public bool Exists(int saveSlot = 1)
        {
            return System.IO.File.Exists(Application.persistentDataPath + "/save-" + saveSlot + ".save");
        }

        public void Clear(int saveSlot = 1)
        {
            if (Exists(saveSlot))
            {
                System.IO.File.Delete(Application.persistentDataPath + "/save-" + saveSlot + ".save");
            }
        }
    }

}