using UnityEngine;
using UnityEngine.Events;

namespace Enjlectric.ScriptableData
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Enjlectric/ScriptableData/Event")]
    public class SharedEvent : ScriptableObject
    {
        [SerializeField] private UnityEvent _event;

        public void Invoke()
        {
            _event.Invoke();
        }

        public void AddListener(UnityAction call)
        {
            _event.AddListener(call);
        }

        public void RemoveListener(UnityAction call)
        {
            _event.RemoveListener(call);
        }

        public void RemoveAllListeners()
        {
            _event.RemoveAllListeners();
        }

        public UnityEventCallState GetPersistentListenerState(int index)
        {
            return _event.GetPersistentListenerState(index);
        }

        public void SetPersistentListenerState(int index, UnityEventCallState state)
        {
            _event.SetPersistentListenerState(index, state);
        }
    }
}