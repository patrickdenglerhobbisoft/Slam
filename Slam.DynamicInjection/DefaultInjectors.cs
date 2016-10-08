using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

using Newtonsoft.Json;
using System.Threading;
using Hobbisoft.Slam.Tools.Analyzers;
using Microsoft.VisualStudio.Shell;

namespace Hobbisoft.Slam.DynamicInjection
{

    public enum VisualizerType { Database = 1 }

    public class DefaultInjectors
    {
        private static DefaultInjectors _singletonDefaultInjectors = null;



        private MessageClient messageClient;
        private MessageClientLog messageClientLog;

        public DefaultInjectors()
        {
            messageClient = new MessageClient();
            messageClientLog = new MessageClientLog();

        }

        public async static void UpdateDBWatcher(string connectionString, SqlCommand cmd)
        {
            if (_singletonDefaultInjectors == null)
            {
                _singletonDefaultInjectors = new DefaultInjectors();
            }
            foreach (SqlParameter p in cmd.Parameters)
            {
                if (p.Value == null)
                {
                    p.Value = DBNull.Value;
                }
            }

            await _singletonDefaultInjectors.messageClient.Send(cmd, connectionString);

        }

        public async static void UpdateLogger(object message, Exception exception = null)
        {
            if (_singletonDefaultInjectors == null)
            {
                _singletonDefaultInjectors = new DefaultInjectors();
            }


            await _singletonDefaultInjectors.messageClientLog.Send(message, exception);

        }
    }




}
