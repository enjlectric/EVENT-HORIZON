using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Enjlectric.ScriptableData
{
    public class SaveInterface : MonoBehaviour
    {
        [SerializeField] private SaveDataManager _manager;

        public void Save(int slot)
        {
            _manager.Save(slot);
        }
        public void Restore(int slot)
        {
            _manager.Restore(slot);
        }
        public bool Exists(int slot)
        {
            return _manager.Exists(slot);
        }
        public void Clear(int slot)
        {
            _manager.Clear(slot);
        }
        public void Reset()
        {
            _manager.Reset();
        }
    }

}