using LightSpace_WPF_Engine.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightSpace_WPF_Engine.Models.Utility
{
    public abstract class CoreBehavior
    {
        private delegate void OnUpdateEvent();
        private readonly OnUpdateEvent onUpdateEvent;
        private delegate void OnLateUpdateEvent();
        private readonly OnLateUpdateEvent onLateUpdateEvent;
        private delegate void OnDestroyEvent();
        private readonly OnDestroyEvent onDestroyEvent;

        protected CoreBehavior()
        {
            BehaviorManager.Get.AddBehavior(this);
            Start();
            onUpdateEvent = Update;
            onLateUpdateEvent = LateUpdate;
            onDestroyEvent = OnDestroy;
        }

        public bool RunEvent(EventState eventState)
        {
            switch (eventState)
            {
                case EventState.None:
                    return true;                
                case EventState.Input:
                    return true;
                case EventState.Start:
                    return true;
                case EventState.Update:
                    onUpdateEvent.Invoke();
                    return true;
                case EventState.LateUpdate:
                    onLateUpdateEvent.Invoke();
                    return true;
                case EventState.Destroy:
                    onDestroyEvent.Invoke();
                    return true;
                default: 
                    return false;
            }
        }

        public virtual void Start() { }

        public virtual void Update() { }

        public virtual void LateUpdate() { }

        public virtual void OnDestroy() { }

        public void Destroy()
        {
            BehaviorManager.Get.RemoveBehavior(this);
        }
    }
}
