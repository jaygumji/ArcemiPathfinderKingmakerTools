using System;
using System.IO;

namespace Arcemi.Models
{
    public abstract class Logger
    {
        protected Logger()
        {
        }

        public static Logger Current { get; private set; } = ConsoleLogger.Instance;

        public static void UseConsole() => Current = ConsoleLogger.Instance;
        public static void UseDeferred(Action<object, LogEventType> writeObject = null)
            => Current = new DeferredLogger(writeObject);

        protected abstract void WriteObject(object obj, LogEventType type);
        protected void WriteException(Exception ex, LogEventType type)
        {
            WriteObject(GetExceptionObject(ex), type);
        }

        private object GetExceptionObject(Exception ex)
        {
            return new {
                ex.Message,
                ex.StackTrace,
                InnerException = ex.InnerException is null ? null : GetExceptionObject(ex)
            };
        }

        public void Information(string message, Exception exception = null)
        {
            WriteObject(message, LogEventType.Information);
            if (exception is object) {
                WriteException(exception, LogEventType.Information);
            }
        }

        public void Warning(string message, Exception exception = null)
        {
            WriteObject(message, LogEventType.Warning);
            if (exception is object) {
                WriteException(exception, LogEventType.Warning);
            }
        }

        public void Error(string message, Exception exception = null)
        {
            WriteObject(message, LogEventType.Error);
            if (exception is object) {
                WriteException(exception, LogEventType.Error);
            }
        }
    }

    public class ConsoleLogger : Logger
    {
        public static ConsoleLogger Instance { get; } = new ConsoleLogger();
        private ConsoleLogger()
        {
            
        }

        protected override void WriteObject(object obj, LogEventType type)
        {
            switch (type) {
                case LogEventType.Warning:
                    Console.Write("Warning: ");
                    break;
                case LogEventType.Error:
                    Console.Write("Error: ");
                    break;
            }

            string message;
            if (obj is null) message = "Logging of <null>";
            else if (obj is string str) message = str;
            else message = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

            Console.WriteLine(message);
        }
    }

    public class DeferredLogger : Logger
    {
        private readonly Action<object, LogEventType> writeObject;

        public DeferredLogger(Action<object, LogEventType> writeObject)
        {
            this.writeObject = writeObject;
        }

        protected override void WriteObject(object obj, LogEventType type)
        {
            writeObject(obj, type);
        }
    }

    public enum LogEventType
    {
        Information,
        Warning,
        Error
    }
}
