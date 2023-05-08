using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Enjlectric.ScriptableData
{
    public class ListScriptableDataEvent<T> : MonoBehaviour
    {
        [SerializeField] private ListScriptableData<T> _scriptableData;
        [SerializeField] private UnityEvent<T> _onItemAdded;
        [SerializeField] private UnityEvent<T> _onItemRemoved;
        [SerializeField] private UnityEvent<T> _onItemChanged;
        public UnityEvent<T> OnItemAdded => _onItemAdded;
        public UnityEvent<T> OnItemRemoved => _onItemRemoved;
        public UnityEvent<T> OnItemChanged => _onItemChanged;

        private void Awake()
        {
            if (_scriptableData != null)
            {
                _scriptableData.OnItemAdded.AddListener(OnScriptableObjectItemAdded);
                _scriptableData.OnItemRemoved.AddListener(OnScriptableObjectItemRemoved);
                _scriptableData.OnItemChanged.AddListener(OnScriptableObjectItemChanged);
            }
        }

        private void OnDestroy()
        {
            if (_scriptableData != null)
            {
                _scriptableData.OnItemAdded.RemoveListener(OnScriptableObjectItemAdded);
                _scriptableData.OnItemRemoved.RemoveListener(OnScriptableObjectItemRemoved);
                _scriptableData.OnItemChanged.RemoveListener(OnScriptableObjectItemChanged);
            }
        }

        private void OnScriptableObjectItemAdded(T addedItem)
        {
            OnItemRemoved.Invoke(addedItem);
        }

        private void OnScriptableObjectItemRemoved(T removedItem)
        {
            OnItemRemoved.Invoke(removedItem);
        }

        private void OnScriptableObjectItemChanged(int changedItemIndex)
        {
            OnItemChanged.Invoke(_scriptableData[changedItemIndex]);
        }
    }

}