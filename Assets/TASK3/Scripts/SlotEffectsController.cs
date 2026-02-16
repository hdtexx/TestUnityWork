using System;
using System.Collections.Generic;
using AxGrid.Base;
using UnityEngine;

namespace TASK3.Scripts
{
    public class SlotEffectsController : MonoBehaviourExtBind
    {
        [SerializeField] private ModelTrigger[] _triggers;

        private readonly List<(string key, Action callback)> _subscriptions = new();

        [OnStart]
        private void Subscribe()
        {
            for (var i = 0; i < _triggers.Length; i++)
            {
                var trigger = _triggers[i];
                var captured = trigger;
                Action callback = () => Evaluate(captured);
                var eventKey = $"On{trigger.TriggerKey}Changed";
                _subscriptions.Add((eventKey, callback));
                Model.EventManager.AddAction(eventKey, callback);
            }
        }

        [OnDestroy]
        private void Unsubscribe()
        {
            for (var i = 0; i < _subscriptions.Count; i++)
            {
                var (key, callback) = _subscriptions[i];
                Model.EventManager.RemoveAction(key, callback);
            }

            _subscriptions.Clear();
        }

        private void Evaluate(ModelTrigger trigger)
        {
            if (Model.GetBool(trigger.TriggerKey) != trigger.TriggerValue)
                return;

            for (var i = 0; i < trigger.OnEnable.Length; i++)
            {
                var go = trigger.OnEnable[i];
                if (go) 
                    go.SetActive(true);
            }

            for (var i = 0; i < trigger.OnDisable.Length; i++)
            {
                var go = trigger.OnDisable[i];
                if (go) 
                    go.SetActive(false);
            }
        }
        
        [Serializable]
        public class ModelTrigger
        {
            public string TriggerKey;
            public bool TriggerValue = true;
            public GameObject[] OnEnable;
            public GameObject[] OnDisable;
        }
    }
}