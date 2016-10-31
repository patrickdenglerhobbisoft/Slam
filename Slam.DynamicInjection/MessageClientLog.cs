#if VSTOOLS
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
using Microsoft.VisualStudio.Shell;
using Slam.Visualizers;
/*
http://msdn.microsoft.com/en-us/library/bb310550.aspx
http://www.theserverside.net/blogs/thread.tss?thread_id=45078

 */

namespace Slam.DynamicInjection
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
                Thread t = new Thread(() =>
                   {
                       Debug.WriteLine("*************** MESSAGE ****************");
                       WaitHandle.SignalAndWait(_messageWait, _messageHandled);

                   });

                t.SetApartmentState(ApartmentState.MTA);
                t.Start();
            }
            return true;
        }

        private string prepareError(object message, Exception exception)
        {
            LoggerRemote lr = new LoggerRemote()
            {
                Message = message.ToString(),
            };
            return JsonConvert.SerializeObject(lr);
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
#endif