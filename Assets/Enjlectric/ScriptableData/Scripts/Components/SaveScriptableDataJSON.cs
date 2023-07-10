using Newtonsoft.Json;
using UnityEngine;

namespace Enjlectric.ScriptableData
{
    /// <summary>
    /// JSON implementation of a Save Data system.
    /// </summary>
    public class SaveScriptableDataJSON : SaveScriptableDataBase
    {
        private string GetFilePath(int saveSlot)
        {
            return $"{Application.persistentDataPath}/{_saveGroup.FileName.Replace("#", saveSlot.ToString())}";
        }

        public override void Restore(int saveSlot)
        {
            try
            {
                if (_scriptableDataMap.Count == 0)
                {
                    BuildScriptableDataMap();
                }
                var file = System.IO.File.ReadAllText(GetFilePath(saveSlot));
                if (file != null)
                {
                    SerializedSaveData sd = JsonConvert.DeserializeObject<SerializedSaveData>(file);
                    for (int i = 0; i < sd.identifiers.Count; i++)
                    {
                        if (_scriptableDataMap.ContainsKey(sd.identifiers[i]))
                        {
                            _scriptableDataMap[sd.identifiers[i]].Restore(JsonConvert.DeserializeObject(sd.values[i], _scriptableDataMap[sd.identifiers[i]].GetValueType()));
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.Log("No valid SaveGame found.");
                Debug.Log(e);
            }
        }

        public override void Save(int saveSlot)
        {
            SerializedSaveData sd = new SerializedSaveData();
            foreach (ScriptableDataBase value in _saveGroup.SaveDataValues)
            {
                var obj = value.GetCurrentValue();
                if (obj != null)
                {
                    sd.identifiers.Add(value.GetIdentifier());
                    sd.values.Add(JsonConvert.SerializeObject(obj));
                }
            }

            try
            {
                System.IO.FileStream file = System.IO.File.Create(GetFilePath(saveSlot));
                System.IO.StreamWriter writer = new System.IO.StreamWriter(file);
                var json = JsonConvert.SerializeObject(sd);
                writer.Write(json);
                writer.Flush();
                file.Close();
            }
            catch (System.Exception e)
            {
                Debug.LogError("Something went wrong saving the Save Data.");
                Debug.LogError(e);
            }
        }

        public override void ResetToDefault()
        {
            foreach (var value in _saveGroup.SaveDataValues)
            {
                value.ResetToDefault();
            }
        }

        public override bool Exists(int saveSlot)
        {
            return System.IO.File.Exists(GetFilePath(saveSlot));
        }

        public override void Clear(int saveSlot)
        {
            if (Exists(saveSlot))
            {
                System.IO.File.Delete(GetFilePath(saveSlot));
            }
        }
    }
}