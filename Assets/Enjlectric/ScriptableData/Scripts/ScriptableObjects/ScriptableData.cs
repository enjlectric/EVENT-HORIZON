using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace Enjlectric.ScriptableData
{
    [System.Serializable]
    public class ScriptableData<T> : ScriptableDataBase
    {
        [Tooltip("This value will be used when the game runs in a build, or when production values are set to be used via the top menu: Tools -> ScriptableData -> UseProductionValues")]
        [HighlightIfUsed(true)]
        [SerializeField] protected T _productionDefaultValue;

        [Tooltip("This value will be used when the game runs in the editor and when production values are NOT set to be used via the top menu: Tools -> ScriptableData -> UseProductionValues")]
        [HighlightIfUsed(false)]
        [SerializeField] protected T _debugDefaultValue;

#if UNITY_EDITOR

        [AutoReset("_productionDefaultValue", "_debugDefaultValue")]
        protected T _currentValue;

#else
        protected T _currentValue;
        private bool _initialized = false;
#endif

        public T Value
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

        public override UnityEvent OnValueChanged { get; set; } = new UnityEvent();

        public void SetValueWithoutNotify(T value)
        {
            _currentValue = value;
#if !UNITY_EDITOR
            if (!_initialized) {
                _initialized = true;
            }
#endif
        }

        public override string GetIdentifier()
        {
            return name;
        }

        public override System.Type GetValueType()
        {
            return typeof(T);
        }

        /// <summary>
        /// Return the current value of the scriptable data.
        /// This should only be necessary to use if your reference is to a ScriptableDataBase. Otherwise, use the .Value property directly.
        /// (!) Since this returns the value boxed as an object, the == operator might misbehave. If you need to compare the return value, make sure to use .Equals instead.
        /// </summary>
        /// <returns>The current value of the Scriptable Object.</returns>
        public override object GetCurrentValue()
        {
            return Value;
        }

        public override void Restore(object valueToRestoreTo)
        {
            Value = (T)valueToRestoreTo;
        }

        public override void ResetToDefault()
        {
#if !UNITY_EDITOR
            Value = _productionDefaultValue;
#else
            if (EditorPrefs.GetBool("ScriptableDataUseProductionValues", false))
            {
                Value = _productionDefaultValue;
            }
            else
            {
                Value = _debugDefaultValue;
            }
#endif
        }
    }
}