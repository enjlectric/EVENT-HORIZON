using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Enjlectric.ScriptableData
{
    public class ConditionalEvent : MonoBehaviour
    {
        public enum ConditionType {
            Equals, NotEqual
        }

        [SerializeField] private ScriptableDataBase _a;
        [SerializeField] private ScriptableDataBase _b;

        [SerializeField] private ConditionType _conditionType;

        [SerializeField] private UnityEvent _onConditionTrue;
        [SerializeField] private UnityEvent _onConditionFalse;

        public UnityEvent OnConditionTrue => _onConditionTrue;
        public UnityEvent OnConditionFalse => _onConditionFalse;

        private bool _isCurrentlyTrue = false;

        private void Start()
        {
            if (_a == null || _b == null)
            {
                return;
            }

            _a.OnValueChanged.AddListener(ReEvaluate);
            _b.OnValueChanged.AddListener(ReEvaluate);

            ReEvaluate(true);
        }

        private void OnDestroy()
        {
            if (_a == null || _b == null)
            {
                return;
            }

            _a.OnValueChanged.RemoveListener(ReEvaluate);
            _b.OnValueChanged.RemoveListener(ReEvaluate);
        }

        private void ReEvaluate()
        {
            ReEvaluate(false);
        }

        private void ReEvaluate(bool bypassIsCurrentlyTrue)
        {
            bool becomesTrue = false;
            switch (_conditionType)
            {
                case ConditionType.Equals:
                    becomesTrue = _a.Equals(_b);
                    break;
                case ConditionType.NotEqual:
                    becomesTrue = _a != _b;
                    break;
            }

            if (bypassIsCurrentlyTrue || (becomesTrue != _isCurrentlyTrue))
            {
                _isCurrentlyTrue = becomesTrue;
                if (becomesTrue)
                {
                    OnConditionTrue?.Invoke();
                } else
                {
                    OnConditionFalse?.Invoke();
                }
            }
        }
    }
}