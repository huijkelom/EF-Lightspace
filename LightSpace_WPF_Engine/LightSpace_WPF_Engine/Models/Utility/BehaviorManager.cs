using LightSpace_WPF_Engine.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightSpace_WPF_Engine.Models.Utility
{
    public class BehaviorManager
    {
        private static BehaviorManager _instance;

        public static BehaviorManager Get => _instance ?? (_instance = new BehaviorManager());

        public List<CoreBehavior> ActiveBehaviors { get; set; }

        private BehaviorManager()
        {
            ActiveBehaviors = new List<CoreBehavior>();
        }

        public void RunEvent(EventState eventState)
        {
            ActiveBehaviors.ForEach(behavior => behavior.RunEvent(eventState));
        }

        public bool AddBehavior(CoreBehavior behavior)
        {
            // Add behavior to list of CoreBehaviors if behavior is not null && does not already exist in the list.
            if(behavior != null && ActiveBehaviors.FirstOrDefault(obj => obj == behavior) == null)
            {
                ActiveBehaviors.Add(behavior);
                return true;
            }
            return false;
        }

        public bool RemoveBehavior(CoreBehavior behavior)
        {
            // Remove behavior from list of CoreBehaviors if behavior is not null && does not already exist in the list.
            if (behavior != null && ActiveBehaviors.FirstOrDefault(obj => obj == behavior) != null)
            {
                ActiveBehaviors.Remove(behavior);
                return true;
            }
            return false;
        }
    }
}
