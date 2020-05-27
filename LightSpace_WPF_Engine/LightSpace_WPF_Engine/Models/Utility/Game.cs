using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightSpace_WPF_Engine.Models.Utility
{
    public sealed class Game
    {
        private static Game _instance;

        public static Game Get => _instance ?? (_instance = new Game());

        public CoreLoop CoreLoop { get; private set; }

        public TileManager TileManager { get; private set; }

        public RunningGameBehavior RunningGameBehavior { get; private set; }

        public Game Init()
        {
            CoreLoop = new CoreLoop();
            TileManager = new TileManager();
            return _instance;
        }

        public void SetRunningGameBehavior(RunningGameBehavior gameBehavior)
        {
            if (HasRunningGame())
            {
                RunningGameBehavior.Destroy();
                RunningGameBehavior = null;
            }
            RunningGameBehavior = gameBehavior;
        }

        public bool HasRunningGame()
        {
            return RunningGameBehavior != null;
        }

        public void ToggleLoop()
        {
            if (CoreLoop.IsRunning)
            {
                CoreLoop.StopLoop();
                TileManager.HardwareController.Stop();
            }
            else
            {
                CoreLoop.StartLoop();
                TileManager.HardwareController.Start();
            }
        }

        public void ShutDown()
        {
            CoreLoop.StopLoop();
            TileManager.HardwareController.Stop();
        }
    }
}