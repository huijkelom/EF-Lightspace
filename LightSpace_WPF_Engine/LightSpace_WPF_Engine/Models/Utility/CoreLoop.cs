using LightSpace_WPF_Engine.Models.Models;
using LightSpace_WPF_Engine.Models.Models.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using LightSpace_WPF_Engine.Wpf.Views.MainWindows;

namespace LightSpace_WPF_Engine.Models.Utility
{
    public sealed class CoreLoop 
    {
        public bool UnrestrictedTickSpeed { get; set; } = false;

        public int TicksPerSecond { get; set; } = 1;

        public bool IsRunning = false;

        public Thread Thread { get; private set; }

        public CoreLoop()
        {

        }

        public void StartLoop()
        {
            IsRunning = true;

            if (Thread != null && Thread.IsAlive)
            {
                StopLoop();
            }

            Thread = new Thread(Loop)
            {
                IsBackground = false,
                Name = "Core Game Loop Thread"
            };
            Thread.Start();
        }

        public void StopLoop()
        {
            IsRunning = false;

            if (Thread != null && Thread.IsAlive)
            {
                Thread.Abort();
            }
            else
            {
                ConsoleLogger.WriteToUiConsole(this, "Trying to stop Loop which is either null or not alive.",true);
            }
        }

        private void Loop()
        {
            try
            {
                var nextGameTick = Time.CurrentTick();

                // nanoseconds / amount of frames per second = amount of ticks to skip for desired FrameRate
                var skipTicks = 10000000 / TicksPerSecond;
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                while (IsRunning)
                {
                    
                    // Keep track of ticks so speed can be calculated
                    var currentTick = Time.CurrentTick();

                    // Do logic events as fast as the set FrameRate
                    if (currentTick >= nextGameTick || UnrestrictedTickSpeed)
                    {
                        MainWindow.Main.GetDispatcher.BeginInvoke(Game.Get.TileManager.HardwareController.Read);
                        Time.SetDeltaTime(stopwatch.ElapsedMilliseconds);
                        // Handle input received from hardware or engine source
                        BehaviorManager.Get.RunEvent(Enums.EventState.Update);
                        BehaviorManager.Get.RunEvent(Enums.EventState.LateUpdate);
                        nextGameTick = skipTicks + currentTick;

                        // Draw after every game tick
                        MainWindow.Main.GetDispatcher.BeginInvoke(MainWindow.Main.RenderDelegate);

                        // Check for and clear all inputs
                        var inputs = Input.GetOutputData(true);
                        if (inputs.Count > 0)
                        {
                            ConsoleLogger.WriteToUiConsole("", $"Inputs received : {inputs.Count}", true);
                        }
                        stopwatch.Restart();
                        MainWindow.Main.GetDispatcher.BeginInvoke(Game.Get.TileManager.HardwareController.Write);
                    }
                }
            }
            catch(Exception exception)
            {
                ConsoleLogger.WriteToConsole(this, "Exception thrown during Core Loop.", exception);
            }            
        }
    }
}