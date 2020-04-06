using System;
using LightSpace_WPF_Engine.Models.Exceptions;
using LightSpace_WPF_Engine.Wpf.ViewModels.Utility;

namespace LightSpace_WPF_Engine.Models.Models.Logging
{
    public class ConsoleMessage
    {
        public long Id { get; private set; }

        public object Data { get; private set; }

        public string Message { get; private set; }

        public string Exception { get; private set; } = "No Exception";

        public short Count { get; set; }

        private long dateTimeTicks;

        public ConsoleMessage(long listSize, object data, string message, Exception exception = null)
        {
            this.Id = listSize;
            this.Data = data;
            this.Message = message;
            if (exception != null)
                this.Exception = exception.ToString();
            this.dateTimeTicks = DateTime.Now.Ticks;
            this.Count = 1;
        }

        public DateTime GetDateTime()
        {
            return new DateTime(dateTimeTicks);
        }

        private void SetLatestDateTime()
        {
            this.dateTimeTicks = DateTime.Now.Ticks;
        }

        public void IncrementCounter(bool updateDateTime)
        {
            Count++;

            if (updateDateTime)
            {
                SetLatestDateTime();
            }
        }

        public override string ToString()
        {
            if (Data != null)
            {
                return string.IsNullOrEmpty(Exception) ?
                    $"{Count}\t[{GetDateTime()}]\t{Message,60}\t{Data.ToString()} [{Exception.ToString()}]" :
                    $"{Count}\t[{GetDateTime()}]\t{Message,60}\t{Data.ToString()}";
            }
            return string.IsNullOrEmpty(Exception) ?
                $"{Count}\t[{GetDateTime()}]\t{Message,60} [{Exception.ToString()}]" :
                $"{Count}\t[{GetDateTime()}]\t{Message,60} ";
        }

        public string ToLoggedString(long entryNumber)
        {
            if (Data != null)
            {
                return string.IsNullOrEmpty(Exception) ?
                    $"{entryNumber,5} [{GetDateTime()}] : ({Data.ToString()}) {Message} [{Exception}]" :
                    $"{entryNumber,5} [{GetDateTime()}] : ({Data.ToString()}) {Message} ";
            }
            return string.IsNullOrEmpty(Exception) ?
                $"{entryNumber,5} [{GetDateTime()}] {Message} [{Exception}]" :
                $"{entryNumber,5} [{GetDateTime()}] {Message} ";
        }
    }
}
