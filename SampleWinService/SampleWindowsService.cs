using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Configuration;
using JITMessageProcessorConsole;
using JITMessageProcessor.Common;

namespace JITMessageProcessingWindowsService
{
    public partial class JITMessageProcessingWindowsService : ServiceBase
    {
        public const int DELAY_BETWEEN_SCANS_SEC_DEFAULT = 900;
        public const string SERVICE_NUMBER_CONFIG = "ServiceNumber";
        public const string DATA_SERVER_NAME_CONFIG = "DataServerName";

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Thread m_Worker;

        private AutoResetEvent m_StopRequest = new AutoResetEvent(false);

        public static int ServiceNumber
        {
            get
            {
                try
                {
                    return int.Parse(ConfigurationManager.AppSettings[SERVICE_NUMBER_CONFIG].ToString());
                }
                catch
                {
                    log.Warn(string.Format("Failed parsing [{0}] field value", SERVICE_NUMBER_CONFIG));

                    return 1;
                }
            }
        }

        public static string DataServerName
        {
            get
            {
                return ConfigurationManager.AppSettings[DATA_SERVER_NAME_CONFIG].ToString();
            }
        }

        private int? m_ScanIntervalSec = null;
        internal int ScanIntervalSec
        {
            get
            {
                if (m_ScanIntervalSec == null)
                {
                    var dbConfig = ProcessorConfig.GetObject(DataServerName, ServiceNumber);
                    this.m_ScanIntervalSec = dbConfig.DelaySecondsBetweenScans;
                    if (!this.m_ScanIntervalSec.HasValue)
                        this.m_ScanIntervalSec = DELAY_BETWEEN_SCANS_SEC_DEFAULT;
                }

                return this.m_ScanIntervalSec.Value;
            }
        }

        public JITMessageProcessingWindowsService()
        {
            //InitializeComponent();
            this.ServiceName = "JITMessageProcessingWindowsService";
            this.CanStop = true;
            this.AutoLog = true;
        }

        protected override void OnStart(string[] args)
        {
            // Start the worker thread
            this.m_Worker = new Thread(DoWork);
            this.m_Worker.Start();
        }

        protected override void OnStop()
        {
            // Signal worker to stop and wait until it does
            this.m_StopRequest.Set();
            this.m_Worker.Join();

            // Triggers to get an updated value for the interval
            this.m_ScanIntervalSec = null;
        }

        public void OnDebug()
        {
            OnStart(null);
        }

        private void DoWork(object arg)
        {
            DataInit.PopulateDataProcessorConfigTable();
            DataInit.PopulateInitialPrefixList();
            DataInit.PopulateZonesTable();

            //run first time
            new JITProcess().StartProcessing();

            //run in a loop after waiting n seconds or stop right away if the service is stopped
            // Worker thread loop
            for (;;)
            {
                // Run this code once every n seconds or stop right away if the service is stopped
                if (this.m_StopRequest.WaitOne(ScanIntervalSec*1000)) return;
                // Do work...
                new JITProcess().StartProcessing();
            }
        }
    }
}
