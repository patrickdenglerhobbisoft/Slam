
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
    class MessageServer
    {
        private MemoryMappedFile _mmf;
        private EventWaitHandle _messageWait;
        private EventWaitHandle _messageHandled;

        private ErrorLog errorLogger = null;
        public static bool _quit;
        private VisualizerHost visualizer;
        private Dictionary<string, SQLViz> DBWatchers = new Dictionary<string, SQLViz>();
        public MessageServer(VisualizerHost visualizer)
        {
            string commName = "commDB";
            _mmf = MemoryMappedFile.CreateOrOpen("mmf" + commName, 256 * 1024);
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
            SqlRemote sqlRemote = null;

            try
            {

                sqlRemote = JsonConvert.DeserializeObject<SqlRemote>(decodedMessage, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, });
                string connectionString = sqlRemote.ConnectionString;


                ThreadHelper.Generic.BeginInvoke(() =>
                {

                    if (!DBWatchers.ContainsKey(connectionString))
                    {
                        SQLViz sqlViz = new SQLViz();
                        sqlViz.ConnectionString = connectionString;
                        DBWatchers.Add(connectionString, sqlViz);
                        visualizer.AddVisualizer(sqlViz as IVisualizer);

                    }
                    DBWatchers[connectionString].AddSQLStatement(sqlRemote);

                });
            }
            catch { }
            return;



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


    [JsonObject(MemberSerialization.OptIn)]
    public class LoggerRemote
    {
        // public object Exception { get; set; }
        [JsonProperty]
        public string Message { get; set; }
    }


    [JsonObject(MemberSerialization.OptIn)]
    public class SqlRemote
    {
        [JsonProperty]
        public List<SqlRemoteParameter> SqlParameters { get; set; }
        [JsonProperty]
        public string ConnectionString { get; set; }
        [JsonProperty]
        public string CommandText { get; set; }
        public SqlRemote()
        {
            SqlParameters = new List<SqlRemoteParameter>();
        }
    }
    [JsonObject(MemberSerialization.OptIn)]
    public class SqlRemoteParameter
    {
        [JsonProperty]
        public string ParameterName { get; set; }
        [JsonProperty]
        public string ParameterValue { get; set; }
    }
}


