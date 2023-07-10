using UnityEngine.Events;

namespace Enjlectric.ScriptableData
{
    public interface IValueChangedHandler
    {
        UnityEvent OnValueChanged { get; set; }
    }
}