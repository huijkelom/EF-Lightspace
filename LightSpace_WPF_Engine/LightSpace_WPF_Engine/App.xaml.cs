using LightSpace_WPF_Engine.Models.Models.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace LightSpace_WPF_Engine
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App() : base()
        {
            this.Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        }

        void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            ConsoleLogger.WriteToConsole(sender, $"Unhandled exception occurred.", e.Exception);

            MessageBox.Show($"Check Log file for more information ({ ConsoleLogger.LogFilePath}). Error: \n" + e.Exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
