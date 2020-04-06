using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using LightSpace_WPF_Engine.Models.Models.Logging;
using LightSpace_WPF_Engine.Wpf.ViewModels.Utility;
using LightSpace_WPF_Engine.Wpf.Views.MainWindows;

namespace LightSpace_WPF_Engine.Wpf.ViewModels
{
    public class CustomConsole : ViewModelBase
    {
        private short MessageLimit { get; } = 100;

        private readonly object lockObject = new object();

        private ObservableCollection<ConsoleMessage> consoleMessages = new ObservableCollection<ConsoleMessage>();

        public ObservableCollection<ConsoleMessage> ConsoleMessages
        {
            get => consoleMessages;
            set
            {
                consoleMessages = value;
                RaisePropertyChanged("ConsoleMessages");
            }
        }

        public delegate void WriteToConsoleEvent(object data, string message, bool unique, Exception exception);
        public WriteToConsoleEvent WriteToConsoleDelegate;

        public CustomConsole()
        {
            WriteToConsoleDelegate = WriteToConsole;
        }

        private void WriteToConsole(object data, string message, bool unique, Exception exception = null)
        {
            lock (lockObject)
            {
                var found = IsMessageDuplicate(data, message);

                if(found != null && !unique)
                {
                    ConsoleMessages.FirstOrDefault(msg => msg.Id == found.Id)?
                        .IncrementCounter(false);
                }
                else
                {
                    if (ConsoleMessages.Count >= MessageLimit)
                    {
                        ConsoleMessages.RemoveAt(0);
                        ConsoleMessages.Add(new ConsoleMessage(ConsoleMessages.Count, data, message, exception));
                        MainWindow.Main.ConsoleGroupBox.Header = $"Console ({ConsoleMessages.Count}) [Log limit reached, overriding oldest Logs]";
                    }
                    else
                    {
                        ConsoleMessages.Add(new ConsoleMessage(ConsoleMessages.Count, data, message, exception));
                        MainWindow.Main.ConsoleGroupBox.Header = $"Console ({ConsoleMessages.Count})";
                    }
                }
            }
            MainWindow.Main.ConsoleListBox.Items.Refresh();
            if (MainWindow.Main.ConsoleListBox.SelectedItem == null)
            {
                MainWindow.Main.ConsoleListBox.ScrollIntoView(
                    MainWindow.Main.ConsoleListBox.Items[MainWindow.Main.ConsoleListBox.Items.Count - 1]
                );
            }
        }

        public ConsoleMessage IsMessageDuplicate(object data, string message)
        {
            lock (lockObject)
            {
                return ConsoleMessages.FirstOrDefault(msg =>
                    (msg.Data.ToString() == data.ToString()
                     && msg.Message == message));
            }
        }

        public List<string> GetAllParsedMessages()
        {
            var messageList = new List<string>();
            lock (lockObject)
            {
                if (consoleMessages.Count != 0)
                {
                    consoleMessages.ToList().ForEach(message => messageList.Add(message.ToString()));
                }
            }
            return messageList;
        }
    }
}
