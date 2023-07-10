using UnityEngine;
using UnityEngine.Events;

namespace Enjlectric.ScriptableData
{
    public abstract class ScriptableDataBase : ScriptableObject, ISaveData, IValueChangedHandler
    {
        public abstract UnityEvent OnValueChanged { get; set; }

        public abstract object GetCurrentValue();

        public abstract string GetIdentifier();

        public abstract System.Type GetValueType();

        public abstract void ResetToDefault();

        public abstract void Restore(object valueToRestoreTo);
    }
}