using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Enjlectric.ScriptableData
{
    [System.Serializable]
    public class ListScriptableData<T> : ScriptableDataBase, IEnumerable<T>
    {
        [Tooltip("This value will be used when the game runs in a build, or when production values are set to be used via the top menu: Tools -> ScriptableData -> UseProductionValues")]
        [InspectorName("Production default value")]
        [SerializeField] protected List<T> _productionDefaultValue = new List<T>();
        [Tooltip("This value will be used when the game runs in the editor and when production values are NOT set to be used via the top menu: Tools -> ScriptableData -> UseProductionValues")]
        [InspectorName("Debug default value")]
        [SerializeField] protected List<T> _debugDefaultValue = new List<T>();

        [AutoReset("_productionDefaultValue", "_debugDefaultValue")]
        protected List<T> v = new List<T>();

        private UnityEvent<T> _onItemAdded = new UnityEvent<T>();
        private UnityEvent<T> _onItemRemoved = new UnityEvent<T>();
        private UnityEvent<int> _onItemChanged = new UnityEvent<int>();

        public UnityEvent<T> OnItemAdded => _onItemAdded;
        public UnityEvent<T> OnItemRemoved => _onItemRemoved;
        public UnityEvent<int> OnItemChanged => _onItemChanged;

        public int Count => Value.Count;

        public List<T> Value
        {
            get { return v; }
            set
            {
                v = value;
                OnValueChanged?.Invoke();
            }
        }

        internal void Add(T item)
        {
            v.Add(item);
            OnItemAdded?.Invoke(item);
        }

        internal void AddUnique(T item)
        {
            if (!v.Contains(item))
            {
                Add(item);
            }
        }

        internal void RemoveAt(int idx)
        {
            if (idx < v.Count && idx >= 0)
            {
                var item = v[idx];
                v.RemoveAt(idx);
                OnItemRemoved?.Invoke(item);
            }
        }

        internal void Remove(T item)
        {
            if (v.Contains(item))
            {
                v.Remove(item);
                OnItemRemoved?.Invoke(item);
            }
        }
        public T this[int key]
        {
            get => Value[key];
            set {
                Value[key] = value;
                OnItemChanged?.Invoke(key);
            }
        }

        internal override void Restore(string saveDataValue)
        {
            Value = JsonConvert.DeserializeObject<List<T>>(saveDataValue);
        }

        internal override object Save()
        {
            return v;
        }

        internal override void ResetToDefault()
        {
#if !UNITY_EDITOR
        Value = _productionDefaultValue;
#else
            if (EditorPrefs.GetBool("ScriptableDataUseProductionValues", false))
            {
                Value = new List<T>(_productionDefaultValue);
            }
            else
            {
                Value = new List<T>(_debugDefaultValue);
            }
#endif
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Value.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Value.GetEnumerator();
        }
        internal override object GetValue()
        {
            return Value;
        }
    }
}