using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.ServiceProcess;
using System.Threading;

namespace ConvertService
{
    class WindowsService : ServiceBase
    {
		protected EventLog _log;
		protected TcpChannel _channel;

        public WindowsService()
        {
            ServiceName = "Teztech Document Conversion";
            EventLog.Source = "Teztech Document Conversion";
            EventLog.Log = "Application";
            
            // These Flags set whether or not to handle that specific
            //  type of event. Set to true if you need it, false otherwise.
            CanHandlePowerEvent = false;
            CanHandleSessionChangeEvent = false;
            CanPauseAndContinue = true;
            CanShutdown = true;
            CanStop = true;

            if (!EventLog.SourceExists("Teztech Document Conversion"))
                EventLog.CreateEventSource("Teztech Document Conversion", "Application");

			_log = new EventLog();
			_log.Source = "Teztech Document Conversion";

			_channel = new TcpChannel(ConvertInterface.ConverterEndpoints.Port);
            ChannelServices.RegisterChannel(_channel, false);

			RemotingConfiguration.RegisterWellKnownServiceType(typeof(DocumentToPdf), ConvertInterface.ConverterEndpoints.Uri, WellKnownObjectMode.Singleton);
        }

        static void Main()
        {
            ServiceBase.Run(new WindowsService());
        }

        /// <summary>
        /// Dispose of objects that need it here.
        /// </summary>
        /// <param name="disposing">Whether or not disposing is going on.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        protected override void OnStart(string[] args)
        {
			_log.WriteEntry("Service Starting");

			_channel.StartListening(null);

            base.OnStart(args);

			_log.WriteEntry("Service Running");
        }

        protected override void OnStop()
        {
			_log.WriteEntry("Service Stopping");

			_channel.StopListening(null);

            base.OnStop();
        }

        protected override void OnPause()
        {
			_channel.StopListening(null);
            base.OnPause();
        }

        protected override void OnContinue()
        {
			_channel.StartListening(null);
            base.OnContinue();
        }
    }
}
