using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        [HighlightIfUsed(true)]
        [SerializeField] protected List<T> _productionDefaultValue = new List<T>();

        [Tooltip("This value will be used when the game runs in the editor and when production values are NOT set to be used via the top menu: Tools -> ScriptableData -> UseProductionValues")]
        [InspectorName("Debug default value")]
        [HighlightIfUsed(false)]
        [SerializeField] protected List<T> _debugDefaultValue = new List<T>();

#if UNITY_EDITOR

        [AutoReset("_productionDefaultValue", "_debugDefaultValue")]
        protected List<T> _currentValue = new List<T>();

#else
        protected List<T> _currentValue;
        private bool _initialized = false;
#endif
        public override UnityEvent OnValueChanged { get; set; } = new UnityEvent();

        private UnityEvent<T> _onItemAdded = new UnityEvent<T>();
        private UnityEvent<T> _onItemRemoved = new UnityEvent<T>();
        private UnityEvent<int> _onItemChanged = new UnityEvent<int>();

        public UnityEvent<T> OnItemAdded => _onItemAdded;
        public UnityEvent<T> OnItemRemoved => _onItemRemoved;
        public UnityEvent<int> OnItemChanged => _onItemChanged;

        public int Count => Value.Count;

        public List<T> Value
        {
            get
            {
#if !UNITY_EDITOR
                if (!_initialized) {
                    _currentValue = _productionDefaultValue;
                    _initialized = true;
                }
#endif
                return _currentValue;
            }
            set
            {
#if !UNITY_EDITOR
                if (!_initialized) {
                    _initialized = true;
                }
#endif
                _currentValue = value;
                OnValueChanged?.Invoke();
            }
        }

        public void Add(T item)
        {
            Value.Add(item);
            OnItemAdded?.Invoke(item);
        }

        public void AddUnique(T item)
        {
            if (!Value.Contains(item))
            {
                Add(item);
            }
        }

        public void RemoveAt(int idx)
        {
            if (idx < Value.Count && idx >= 0)
            {
                var item = Value[idx];
                Value.RemoveAt(idx);
                OnItemRemoved?.Invoke(item);
            }
        }

        public void Remove(T item)
        {
            if (Value.Contains(item))
            {
                Value.Remove(item);
                OnItemRemoved?.Invoke(item);
            }
        }

        public T this[int key]
        {
            get => Value[key];
            set
            {
                Value[key] = value;
                OnItemChanged?.Invoke(key);
            }
        }

        public override string GetIdentifier()
        {
            return name;
        }

        public override object GetCurrentValue()
        {
            return Value;
        }

        public override System.Type GetValueType()
        {
            return typeof(T);
        }

        public override void Restore(object valueToRestoreTo)
        {
            Value = (List<T>)valueToRestoreTo;
        }

        public override void ResetToDefault()
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
    }
}