using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightSpace_WPF_Engine.Models.Utility
{
    [Serializable]
    public class Vector3
    {
        public int X { get; set; }

        public int Y { get; set; }

        public int Z { get; set; }

        public Vector3()
        {
            X = 0;
            Y = 0;
            Z = 0;
        }

        public Vector3(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Vector3 Zero()
        {
            return new Vector3();
        }

        public static Vector3 One()
        {
            return new Vector3(1,1,1);
        }
    }
}
