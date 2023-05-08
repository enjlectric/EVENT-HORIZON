using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Enjlectric.ScriptableData
{
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