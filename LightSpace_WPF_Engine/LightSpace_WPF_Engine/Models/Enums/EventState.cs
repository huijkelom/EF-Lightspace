using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightSpace_WPF_Engine.Models.Enums
{
    public enum EventState
    {
        None = 0,
        Input = 2,

        //CoreBehavior States
        Start = 10,
        Update = 11,
        LateUpdate = 12,
        Destroy = 15

    }
}
