/*
 * WCF Self hosting sample from www.WayneCliffordBarker.co.za
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;
using System.ServiceProcess;

namespace HostHelper
{
    /// <summary>
    /// Abstract class to be used when wanting to host multiple WCF services within a windows service process.
    /// </summary>
    /// <remarks>InstallUtil asselmblyname.exe InstallUtil /u asselmblyname.exe</remarks>
    public abstract class WindowsServiceWCFHostManager : ServiceBase
    {
        private ServiceHostManager serviceManager;

        public abstract string WindowsServiceName { get; }

        public void Start()
        {
            serviceManager.Hosts.AddRange(RegisterServiceHosts());
            serviceManager.OpenHosts();
        }

        protected WindowsServiceWCFHostManager()
        {
            this.ServiceName = this.WindowsServiceName;
            serviceManager = new ServiceHostManager(this.ServiceName);
        }

        public abstract IEnumerable<ServiceHost> RegisterServiceHosts();

        protected override void OnStart(string[] args)
        {
            Start();
        }
        protected override void OnStop()
        {
            serviceManager.CloseHosts();
        }
    }
}
