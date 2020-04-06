using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LightSpace_WPF_Engine.Models.Utility
{
    public static class CustomDebugger
    {
        [DllImport("kernel32.dll")]
        static extern void RaiseException(uint dwExceptionCode, uint dwExceptionFlags, uint nNumberOfArguments, IntPtr lpArguments);

        public static void ForceCrash()
        {
            RaiseException(13, 0, 0, new IntPtr(1));
        }
    }
}
