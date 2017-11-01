using NLog;
using RumblingRhino.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace RumblingRhino.Util
{
    public class NoLogger : IDisposable
    {
        private Logger _logger;
        private DateTime _start;

        /// <summary>
        /// Instantiates a Logger for an instance of the given class.
        /// </summary>
        /// <param name="name">The name of the class to instantiate a logger for.</param>
        public NoLogger(string name)
        {
            _logger = LogManager.GetLogger(name);
            //if (ApplicationSettings.Instance.Debug)
            //{
            //    _start = DateTime.Now;
            //    writeMessage(string.Format("{0} logger started", _logger.Name), LogLevel.Debug);
            //}
        }

        /// <summary>
        /// Is equivalent to calling LogManager.GetCurrentClassLogger. Instantiates an instance of Logger for the current class.
        /// </summary>
        public NoLogger()
        {
            _logger = LogManager.GetLogger(new StackTrace(true).GetFrame(1).GetMethod().DeclaringType.FullName);
            if (ApplicationSettings.Instance.Debug)
            {
                //_start = DateTime.Now;
                //writeMessage(string.Format("{0} logger started", _logger.Name), LogLevel.Debug);
            }
        }

        private void writeMessage(string message, LogLevel level, Exception exception = null)
        {
            //create log event from the passed message                   
            LogEventInfo logEvent = new LogEventInfo(level, _logger.Name, message) { Exception = exception };

            // Call the Log() method. It is important to pass typeof(NoLog) as the            
            // first parameter. If you don't, ${callsite} and other callstack-related             
            // layout renderers will not work properly.             
            _logger.Log(typeof(NoLogger), logEvent);
        }

        public void Trace(string message, params object[] args)
        {
            writeMessage(string.Format(message, args), LogLevel.Trace);
        }

        public void TraceException(string message, Exception exception)
        {
            writeMessage(message, LogLevel.Trace, exception);
        }

        public void Info(string message, params object[] args)
        {
            writeMessage(string.Format(message, args), LogLevel.Info);
        }

        public void InfoException(string message, Exception exception)
        {
            writeMessage(message, LogLevel.Info, exception);
        }

        public void Debug(string message, params object[] args)
        {
            if (ApplicationSettings.Instance.Debug)
                writeMessage(string.Format(message, args), LogLevel.Debug);
        }

        /// <summary>
        /// <para>Used if you want to do more advanced stuff in your debugging</para>
        /// <para>Example: </para>
        /// <para>List&lt;string&gt; strings = new List&lt;string&gt;() {"test", "Hest", "Mooo"};</para>
        /// <para>Func&lt;List&lt;string&gt;, string&gt; func = delegate(List&lt;string&gt; list)</para>
        /// <para>{</para>
        /// <para>     List&lt;string&gt; hest = list.OrderBy(item => item).ToList();</para>
        /// <para>     return string.Join(", ", hest); </para>
        /// <para>};</para>
        /// <para>logger.Debug(func, strings);</para>
        /// </summary>
        /// <param name="method">A Func to execute</param>
        /// <param name="args">Arguments to parse into the Func</param>
        public void Debug(Delegate method, params object[] args)
        {
            if (ApplicationSettings.Instance.Debug)
                writeMessage(method.DynamicInvoke(args).ToString(), LogLevel.Debug);
        }

        /// <summary>
        /// <para>Used if you want to debug stuff that can be contained in a single lambda statement</para>
        /// <para>Example:</para>
        /// <para>logger.Debug(() => string.Join(", ", ListOfStrings.OrderBy(item => item).ToList()));</para>
        /// </summary>
        /// <param name="lambda">The lambda expression to execute</param>
        public void Debug(Func<string> lambda)
        {
            if (ApplicationSettings.Instance.Debug)
                writeMessage(lambda(), LogLevel.Debug);
        }

        public void DebugException(string message, Exception exception)
        {
            if (ApplicationSettings.Instance.Debug)
                writeMessage(message, LogLevel.Debug, exception);
        }

        public void Warn(string message, params object[] args)
        {
            writeMessage(string.Format(message, args), LogLevel.Warn);
        }

        public void WarnException(string message, Exception exception)
        {
            writeMessage(message, LogLevel.Warn, exception);
        }

        public void Error(string message, params object[] args)
        {
            writeMessage(string.Format(message, args), LogLevel.Error);
        }

        public void ErrorException(string message, Exception exception)
        {
            writeMessage(message, LogLevel.Error, exception);
        }

        public void Fatal(string message, params object[] args)
        {
            writeMessage(string.Format(message, args), LogLevel.Fatal);
        }

        public void FatalException(string message, Exception exception)
        {
            writeMessage(message, LogLevel.Fatal, exception);
        }

        public void Dispose()
        {
            if (ApplicationSettings.Instance.Debug)
            {
                writeMessage(string.Format("{0} logger disposed - execution time : {1}", _logger.Name, (DateTime.Now - _start).ToString()), LogLevel.Debug);
            }
        }
    }
}
