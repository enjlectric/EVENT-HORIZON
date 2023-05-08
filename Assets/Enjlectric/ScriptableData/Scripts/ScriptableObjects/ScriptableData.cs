using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Events;
using System.ComponentModel;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Enjlectric.ScriptableData
{
    [System.Serializable]
    public class ScriptableData<T> : ScriptableDataBase
    {
        [Tooltip("This value will be used when the game runs in a build, or when production values are set to be used via the top menu: Tools -> ScriptableData -> UseProductionValues")]
        [SerializeField] protected T _productionDefaultValue;
        [Tooltip("This value will be used when the game runs in the editor and when production values are NOT set to be used via the top menu: Tools -> ScriptableData -> UseProductionValues")]
        [SerializeField] protected T _debugDefaultValue;

        [AutoReset("_productionDefaultValue", "_debugDefaultValue")]
        protected T v;

        public T Value
        {
            get { return v; }
            set
            {
                v = value;
                OnValueChanged?.Invoke();
            }
        }

        public void SetValueWithoutNotify(T value)
        {
            v = value;
        }

        internal override void Restore(string saveDataValue)
        {
            Value = JsonConvert.DeserializeObject<T>(saveDataValue);
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
                Value = _productionDefaultValue;
            }
            else
            {
                Value = _debugDefaultValue;
            }
#endif
        }

        public override bool Equals(object b)
        {
            if (b.GetType() != typeof(string)) { 
                if (b == default)
                {
                    return false;
                }
            }
            if (b is ScriptableDataBase bBase)
            {
                return Value.Equals(bBase.GetValue());
            }
            return Value.Equals(b);
        }

        public override int GetHashCode()
        {
            if (Value == null)
            {
                return 0;
            }
            return Value.GetHashCode();
        }

        internal override object GetValue()
        {
            return Value;
        }
    }

    public class ScriptableDataBase : ScriptableObject
    {
        private UnityEvent _onValueChanged = new UnityEvent();
        public UnityEvent OnValueChanged => _onValueChanged;

        internal virtual void Restore(string saveDataValue)
        {
        }

        internal virtual object Save()
        {
            return null;
        }

        internal virtual void ResetToDefault()
        {

        }

        internal virtual object GetValue()
        {
            return null;
        }
    }
}