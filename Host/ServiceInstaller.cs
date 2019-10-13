using SelfHosting.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace SelfHosting
{
    [RunInstaller(true)]
    public partial class ServiceInstaller : System.Configuration.Install.Installer
    {
        private ServiceProcessInstaller process;
        private System.ServiceProcess.ServiceInstaller service;

        public ServiceInstaller()
        {
            process = new ServiceProcessInstaller();
            process.Account = ServiceAccount.NetworkService;
            service = new System.ServiceProcess.ServiceInstaller();
            service.ServiceName = Resources.ServiceName;
            service.Description = Resources.Description;
            Installers.Add(process);
            Installers.Add(service);
        }
    }
}
