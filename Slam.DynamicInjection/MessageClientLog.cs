using Hobbisoft.Slam.Tools.Analyzers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Hobbisoft.Slam.Tools.Analyzers;
using Microsoft.VisualStudio.Shell;
/*
http://msdn.microsoft.com/en-us/library/bb310550.aspx
http://www.theserverside.net/blogs/thread.tss?thread_id=45078

 */

namespace Hobbisoft.Slam.DynamicInjection
{
    class MessageClientLog
    {
        private MemoryMappedFile _mmf;
        private EventWaitHandle _messageWait;
        private EventWaitHandle _messageHandled;



        public MessageClientLog()
        {
            string commName = "commLog";
            _mmf = MemoryMappedFile.CreateOrOpen("mmf" + commName, 256 * 1024);
            _messageWait = new EventWaitHandle(false, EventResetMode.AutoReset, "wait" + commName);
            _messageHandled = new EventWaitHandle(false, EventResetMode.AutoReset, "handled" + commName);
        }

        public async Task<bool> Send(object message, Exception exception)
        {
            string Message = prepareError(message, exception);
            await SendMessage(Message);
            return true;

        }

        public async Task<bool> Send(SqlCommand command, string SqlConnection)
        {
            string Message = prepareSql(command, SqlConnection);
            await SendMessage(Message);
            return true;

        }

        private async Task<bool> SendMessage(string Message)
        {
            int bufferSize = Message.Length * sizeof(Char);
            byte[] message = new byte[bufferSize];
            UTF8Encoding encoding = new UTF8Encoding();
            encoding.GetBytes(Message, 0, Message.Length, message, 0);

            using (var viewStream = _mmf.CreateViewStream())
            {

                viewStream.Position = 0;
                viewStream.Write(message, 0, bufferSize);
                WaitHandle.SignalAndWait(_messageWait, _messageHandled);

            }

            return true;
        }

        private string prepareError(object message, Exception exception)
        {
            LoggerRemote lr = new Tools.Analyzers.LoggerRemote()
            {
                Message = message.ToString(),
            };
            return JsonConvert.SerializeObject(lr);
        }


        private string prepareSql(SqlCommand command, string SqlConnection)
        {
            SqlRemote sqlRemote = new SqlRemote();

            foreach (SqlParameter p in command.Parameters)
            {
                sqlRemote.SqlParameters.Add(new SqlRemoteParameter()
                {
                    ParameterName = p.ParameterName,
                    ParameterValue = p.Value == null ? "null" : p.Value.ToString(),
                });
            }
            sqlRemote.ConnectionString = SqlConnection;
            sqlRemote.CommandText = command.CommandText;
            return JsonConvert.SerializeObject(sqlRemote);
        }



        public void Close()
        {
            _mmf.Dispose();
            _messageHandled.Set();
            _messageHandled.Dispose();
            _messageWait.Dispose();
        }

        public void Signal()
        {
            _messageHandled.Set();
        }
    }
}
