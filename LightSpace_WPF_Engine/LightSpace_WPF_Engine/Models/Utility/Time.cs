using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightSpace_WPF_Engine.Models.Utility
{
    public static class Time
    {
        public static float DeltaTime { get; private set; }
        public static int FrameRate { get; private set; }

        public static long CurrentTick()
        {
            return DateTime.Now.Ticks;
        }

        public static void SetDeltaTime(long milliseconds)
        {
            // in this case DeltaTime would be nanoseconds between ticks.
            DeltaTime = milliseconds * 0.001f;
            if(DeltaTime != 0 && Math.Abs(DeltaTime) > .0000000f)
            {
                FrameRate = Convert.ToInt32(1 / DeltaTime);
            }
            else
            {
                DeltaTime = .001f;
                FrameRate = 1000;
            }
        }

        public static DateTime GetDateTimeFromTicks(long ticks)
        {
            return new DateTime(ticks);
        }
    }
}
