using LightSpace_WPF_Engine.Models.Enums;
using LightSpace_WPF_Engine.Models.Models.Logging;
using LightSpace_WPF_Engine.Models.Utility;
using LightSpace_WPF_Engine.Wpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace LightSpace_WPF_Engine.Wpf.Views.UserControls
{
    /// <summary>
    /// Interaction logic for PerformanceUserControl.xaml
    /// </summary>
    public partial class PerformanceUserControl : UserControl
    {
        public delegate void AddProfilingDataEvent(int value,PerformanceType performanceType);
        public AddProfilingDataEvent AddFrameRateData;
        public AddProfilingDataEvent AddCpuData;
        public AddProfilingDataEvent AddRamData;

        private readonly GraphViewModel graphViewModel = new GraphViewModel();

        private readonly PerformanceProfiler performanceProfiler;

        public PerformanceUserControl()
        {
            InitializeComponent();
            AddFrameRateData = AddValue;
            AddCpuData = AddValue;
            AddRamData = AddValue;

            performanceProfiler = new PerformanceProfiler(this);

            FrameRatePlot.Visibility = Visibility.Visible;
            FrameRateSeries.ItemsSource = graphViewModel.FramePoints;
            CpuSeries.ItemsSource = graphViewModel.CpuPoints;
            RamSeries.ItemsSource = graphViewModel.RamPoints;
        }

        public void AddValue(int value, PerformanceType performanceType)
        {
            switch (performanceType)
            {
                case PerformanceType.FrameRate:
                    if(value >= 1000)
                    {
                        HighFrameRateOverlay.Visibility = Visibility.Visible;
                        FrameRatePlot.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        HighFrameRateOverlay.Visibility = Visibility.Hidden;
                        FrameRatePlot.Visibility = Visibility.Visible;
                    }

                    graphViewModel.AddPoint(value, performanceType);
                    if(graphViewModel.FramePoints.Count > 10)
                    {
                        graphViewModel.FramePoints.RemoveAt(0);
                    }
                    FrameRateCheckBox.Content = $"Frame rate ({value})";
                    break;
                case PerformanceType.CPU_Usage:
                    graphViewModel.AddPoint(value, performanceType);
                    if (graphViewModel.CpuPoints.Count > 10)
                    {
                        graphViewModel.CpuPoints.RemoveAt(0);
                    }
                    CpuCheckBox.Content = $"CPU Usage ( {value}% )";
                    break;
                case PerformanceType.RAM_Usage:
                    graphViewModel.AddPoint(value, performanceType);
                    if (graphViewModel.RamPoints.Count > 10)
                    {
                        graphViewModel.RamPoints.RemoveAt(0);
                    }
                    RamCheckBox.Content = $"RAM Usage ({value}MB)";
                    break;
            }
        }

        private void FrameRateCheckBox_Click(object sender, RoutedEventArgs e)
        {
            ToggleProfiling();
            performanceProfiler.CheckFrameRate = FrameRateCheckBox.IsChecked ?? false;
            HighFrameRateOverlay.Visibility = Visibility.Hidden;
            FrameRatePlot.Visibility = Visibility.Visible;
            FrameRateCheckBox.Content = $"Frame rate";
        }

        private void CpuCheckBox_Click(object sender, RoutedEventArgs e)
        {
            ToggleProfiling();
            performanceProfiler.CheckCpu = CpuCheckBox.IsChecked ?? false;
            CpuCheckBox.Content = $"CPU Usage ( % )";
        }

        private void RamCheckBox_Click(object sender, RoutedEventArgs e)
        {
            ToggleProfiling();
            performanceProfiler.CheckRam = RamCheckBox.IsChecked ?? false;
            RamCheckBox.Content = $"RAM Usage (MB)";
        }

        private void ToggleProfiling()
        {
            if (AreAnyProfilersActive())
            {
                performanceProfiler.StartProfiling();
            }
            else
            {
                performanceProfiler.StopProfiling();
            }
        }

        private bool AreAnyProfilersActive()
        {
            if (FrameRateCheckBox.IsChecked.GetValueOrDefault(false) || 
                RamCheckBox.IsChecked.GetValueOrDefault(false) || 
                CpuCheckBox.IsChecked.GetValueOrDefault(false))
            {
                return true;
            }
            return false;
        }
    }
}
