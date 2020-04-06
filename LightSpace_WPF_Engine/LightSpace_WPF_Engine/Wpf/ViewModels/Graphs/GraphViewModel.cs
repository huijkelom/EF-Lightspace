using LightSpace_WPF_Engine.Models.Enums;
using LightSpace_WPF_Engine.Wpf.ViewModels.Utility;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightSpace_WPF_Engine.Wpf.ViewModels
{
    class GraphViewModel : ViewModelBase
    {
        public string Title { get; private set; }

        public int AmountOfShownPoints = 30;

        public ObservableCollection<DataPoint> FramePoints { get; private set; } = new ObservableCollection<DataPoint>();
        public ObservableCollection<DataPoint> CpuPoints { get; private set; } = new ObservableCollection<DataPoint>();
        public ObservableCollection<DataPoint> RamPoints { get; private set; } = new ObservableCollection<DataPoint>();

        private static int _frameIndex = 0;
        private static int _cpuIndex = 0;
        private static int _ramIndex = 0;

        public GraphViewModel()
        {

        }

        public void AddPoint(double value, PerformanceType performanceType)
        {
            switch (performanceType)
            {
                case PerformanceType.FrameRate:
                    
                    FramePoints.Add(new DataPoint(_frameIndex, value));
                    _frameIndex++;
                    break;
                case PerformanceType.CPU_Usage:
                    CpuPoints.Add(new DataPoint(_cpuIndex, value));
                    _cpuIndex++;
                    break;
                case PerformanceType.RAM_Usage:
                    RamPoints.Add(new DataPoint(_ramIndex, value));
                    _ramIndex++;
                    break;
            }
            CheckLength(performanceType);
        }

        public void ClearPoints(PerformanceType performanceType)
        {
            switch (performanceType)
            {
                case PerformanceType.FrameRate:
                    FramePoints.Clear();
                    _frameIndex = 0;
                    break;
                case PerformanceType.CPU_Usage:
                    CpuPoints.Clear();
                    _cpuIndex = 0;
                    break;
                case PerformanceType.RAM_Usage:
                    RamPoints.Clear();
                    _ramIndex = 0;
                    break;
            }
            CheckLength(performanceType);
        }

        private void CheckLength(PerformanceType performanceType)
        {
            switch (performanceType)
            {
                case PerformanceType.FrameRate:
                    if (FramePoints.Count > AmountOfShownPoints)
                    {
                        FramePoints.RemoveAt(0);
                    }
                    RaisePropertyChanged("FramePoints");
                    break;
                case PerformanceType.CPU_Usage:
                    if (CpuPoints.Count > AmountOfShownPoints)
                    {
                        CpuPoints.RemoveAt(0);
                    }
                    RaisePropertyChanged("CpuPoints");
                    break;
                case PerformanceType.RAM_Usage:
                    if (RamPoints.Count > AmountOfShownPoints)
                    {
                        RamPoints.RemoveAt(0);
                    }
                    RaisePropertyChanged("RamPoints");
                    break;
            }
        }
    }
}
