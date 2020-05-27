using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using LightSpace_WPF_Engine.Wpf.Views.MainWindows;

namespace LightSpace_WPF_Engine.Models.Models.Logging
{
    public static class ConsoleLogger
    {
        public static bool SaveExceptionsOnly { get; set; } = true;

        public static string LogFilePath { get; set; } = "";

        private static long _entries;

        private static ConsoleMessage _consoleMessage;

        private static string _stringMessage;

        private static FileStream _fileStream;

        private static StreamWriter _streamWriter;

        private static object _lock = new object();

        static ConsoleLogger()
        {
            if (LogFilePath == "")
            {
                LogFilePath = GetFullFilePath();
            }
        }

        public static void Init()
        {
            Console.Title = $"LightSpace_Engine v.{GetPublishedVersion()} [Technical console window]";
        }

        public static void WriteToConsole(object data, string message, Exception exception = null)
        {
            _consoleMessage = new ConsoleMessage(0, data, message, exception);

            _stringMessage = _consoleMessage.ToLoggedString(_entries);
            Console.WriteLine(_stringMessage);
            _entries++;

            if (!SaveExceptionsOnly || exception != null)
            {
                WriteConsoleToFile($"{_stringMessage} {exception.StackTrace}");
            }
        }

        public static void WriteToUiConsole(object data, string message, bool unique, Exception exception = null)
        {
            WriteToConsole(data, message, exception);

            MainWindow.Main.GetDispatcher.BeginInvoke(MainWindow.Main.CustomConsole.WriteToConsoleDelegate, data, message, unique, exception);
        }

        private static void WriteConsoleToFile(string logText)
        {
            lock (_lock)
            {
                _fileStream = new FileStream(LogFilePath, FileMode.Append);
                _streamWriter = new StreamWriter(_fileStream);

                _streamWriter.WriteLine(logText);

                _streamWriter.Flush();
                _streamWriter.Close();
                _fileStream.Close();
            }
        }

        private static string GetFullFilePath()
        {
            var logFilePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}" +
                                $"\\LightSpace_WPF_Engine";
            Directory.CreateDirectory(logFilePath);
            if (LogFilePath != "")
            {
                return LogFilePath;
            }

            return $"{logFilePath}\\LOG_{DateTime.Now:MM_dd_yy_HH_mm_ss_ffff}.txt";
        }

        private static string GetPublishedVersion()
        {
            if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
            {
                return System.Deployment.Application.ApplicationDeployment.CurrentDeployment.
                    CurrentVersion.ToString();
            }
            return "(Not deployed)";
        }
    }
}
