

using Microsoft.VisualStudio.Shell;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;
using System.Threading;

//using TaskDependentEnvDTEProjects = System.Threading.Tasks.Task<System.Collections.Generic.IDictionary<string, System.Collections.Generic.List<EnvDTE.Project>>>;

namespace Hobbisoft.Slam.Tools.Analyzers
{
    class MessageServerLog
    {
        private MemoryMappedFile _mmf;
        private EventWaitHandle _messageWait;
        private EventWaitHandle _messageHandled;

        private ErrorLog errorLogger = null;
        public static bool _quit;
        private VisualizerHost visualizer;
        private Dictionary<string, SQLViz> DBWatchers = new Dictionary<string, SQLViz>();

        public MessageServerLog(VisualizerHost visualizer)
        {
            string commName = "commLog";
            _mmf = MemoryMappedFile.CreateOrOpen("mmf" + commName, 256*1024);
            _messageWait = new EventWaitHandle(false, EventResetMode.AutoReset, "wait" + commName);
            _messageHandled = new EventWaitHandle(false, EventResetMode.AutoReset, "handled" + commName);

            this.visualizer = visualizer;

        }



        public void Monitor()
        {
            var viewStream = _mmf.CreateViewStream();
            StreamReader sr = new StreamReader(viewStream);

            while (!_quit)
            {
                _messageWait.WaitOne(50);
                if (_quit) break;
                try
                {
                    viewStream.Position = 0;
                    var message = sr.ReadToEnd();
                    if (!message.Contains("\0\0\0\0\0\0\0\0\0\0\0\0\0"))
                    { 
                    Decompose(message);
                    _messageHandled.Set();
                        }
                }
                catch (Exception)
                {
                }
               
            }
        }

        private async void Decompose(string decodedMessage)
        {
            LoggerRemote loggerRemote = null;
            loggerRemote = JsonConvert.DeserializeObject<LoggerRemote>(decodedMessage, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, });
            ThreadHelper.Generic.BeginInvoke(() =>
            {
                if (errorLogger == null)
                {
                    errorLogger = new ErrorLog();
                    visualizer.AddVisualizer(errorLogger as IVisualizer);
                }
                errorLogger.AddStatement(loggerRemote);
            });
        }



        public void Close()
        {
            _quit = true;
            _mmf.Dispose();
            _messageWait.Set();
            _messageHandled.Dispose();
            _messageWait.Dispose();
        }
    }


}


