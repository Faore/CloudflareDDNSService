using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Faore.CFService
{
    public enum ServiceState
    {
        SERVICE_STOPPED = 0x00000001,
        SERVICE_START_PENDING = 0x00000002,
        SERVICE_STOP_PENDING = 0x00000003,
        SERVICE_RUNNING = 0x00000004,
        SERVICE_CONTINUE_PENDING = 0x00000005,
        SERVICE_PAUSE_PENDING = 0x00000006,
        SERVICE_PAUSED = 0x00000007,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ServiceStatus
    {
        public int dwServiceType;
        public ServiceState dwCurrentState;
        public int dwControlsAccepted;
        public int dwWin32ExitCode;
        public int dwServiceSpecificExitCode;
        public int dwCheckPoint;
        public int dwWaitHint;
    };

    public partial class CloudflareService : ServiceBase
    {
        ServiceStatus serviceStatus;

        public CloudflareService()
        {
            InitializeComponent();
            this.eventLog = new EventLog();
            if(!EventLog.SourceExists("Cloudflare DDNS Service"))
            {
                EventLog.CreateEventSource(
                "Cloudflare DDNS Service", "Application");
            }
            eventLog.Source = "Cloudflare DDNS Service";
            eventLog.Log = "Application";
        }

        protected override void OnStart(string[] args)
        {
            serviceStatus = new ServiceStatus
            {
                dwCurrentState = ServiceState.SERVICE_START_PENDING,
                dwWaitHint = 1000
            };
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
            //Check the registry for configuration information and ensure the data is correct.
            RegistryKey rootKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Faore\CloudflareService");
            if(rootKey == null)
            {
                eventLog.WriteEntry("No configuration present. Creating skeleton.", EventLogEntryType.Warning);
                rootKey = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Faore\CloudflareService");
            }
            var config = new Configuration(rootKey);


            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        protected override void OnStop()
        {
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOP_PENDING;
            serviceStatus.dwWaitHint = 1000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);

    }
}
