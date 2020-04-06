using LightSpace_WPF_Engine.Models.Enums;
using LightSpace_WPF_Engine.Wpf.Views.UserControls;
using System;
using System.Diagnostics;
using System.Timers;
using LightSpace_WPF_Engine.Models.Models.Logging;

namespace LightSpace_WPF_Engine.Models.Utility
{
    internal class PerformanceProfiler
    {
        public int RefreshRate = 1;
        public bool CheckFrameRate = false;
        public bool CheckCpu = false;
        public bool CheckRam = false;

        private Timer timer;
        private readonly PerformanceUserControl performanceUserControl;

        private double frameRate;
        private double cpuUsage;
        private double ramUsage;

        private readonly PerformanceCounter cpuCounter;
        private readonly PerformanceCounter ramCounter;

        public PerformanceProfiler(PerformanceUserControl performanceUserControl)
        {
            this.performanceUserControl = performanceUserControl;
            this.cpuCounter = new PerformanceCounter("Process", "% Processor Time", Process.GetCurrentProcess().ProcessName);
            this.ramCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");
        }

        public void StartProfiling()
        {
            try
            {
                timer.Close();
            }
            catch (Exception exception)
            {
                if(timer != null)
                    ConsoleLogger.WriteToConsole(timer,$"Error trying to release timer.",exception);
            }

            // refresh rate * nanoseconds (1000)
            timer = new Timer(RefreshRate * 1000)
            {
                AutoReset = true
            };
            timer.Elapsed += Profile;
            timer.Start();
        }

        public void StopProfiling()
        {
            if (timer == null || !timer.Enabled)
            {
                return;
            }
            timer.Stop();
            timer.Dispose();
        }

        private void Profile(object source, ElapsedEventArgs e)
        {
            if (CheckFrameRate)
            {
                frameRate = Time.FrameRate;
                performanceUserControl.Dispatcher?.BeginInvoke(performanceUserControl.AddFrameRateData, (int)frameRate, PerformanceType.FrameRate);
            }

            if (CheckCpu)
            {
                cpuUsage = cpuCounter.NextValue() / Environment.ProcessorCount;
                performanceUserControl.Dispatcher?.BeginInvoke(performanceUserControl.AddCpuData, (int)cpuUsage, PerformanceType.CPU_Usage);
            }

            if (CheckRam)
            {
                ramUsage = ramCounter.NextValue();
                performanceUserControl.Dispatcher?.BeginInvoke(performanceUserControl.AddRamData, (int)ramUsage, PerformanceType.RAM_Usage);
            }
        }
    }
}
