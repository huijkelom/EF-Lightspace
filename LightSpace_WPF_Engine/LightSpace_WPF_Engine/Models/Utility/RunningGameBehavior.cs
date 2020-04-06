using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightSpace_WPF_Engine.Models.Enums;

namespace LightSpace_WPF_Engine.Models.Utility
{
    public class RunningGameBehavior : CoreBehavior
    {
        public static GameName GameName;
        public static string Description;
        public Vector2 GameFieldTileSize;
    }
}
