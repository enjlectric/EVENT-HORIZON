using UnityEngine;
using UnityEngine.Events;

namespace Enjlectric.ScriptableData
{
    /// <summary>
    /// A component which fires unity events when a Scriptable Data of a given type changes. Since Unity doesn't allow generic components, attach one of the derived classes.
    /// </summary>
    /// <typeparam name="T">The type of data.</typeparam>
    public class ScriptableDataEvent<T> : MonoBehaviour
    {
        [SerializeField] private ScriptableData<T> _scriptableData;
        [SerializeField] private UnityEvent<T> _onValueChanged;
        public UnityEvent<T> OnValueChanged => _onValueChanged;

        private void Start()
        {
            if (_scriptableData != null)
            {
                _scriptableData.OnValueChanged.AddListener(OnScriptableObjectValueChanged);
            }

            OnScriptableObjectValueChanged();
        }

        private void OnDestroy()
        {
            if (_scriptableData != null)
            {
                _scriptableData.OnValueChanged.RemoveListener(OnScriptableObjectValueChanged);
            }
        }

        private void OnScriptableObjectValueChanged()
        {
            OnValueChanged.Invoke(_scriptableData.Value);
        }
    }
}