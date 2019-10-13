/*
 * WCF Self hosting sample from www.WayneCliffordBarker.co.za
*/

using HostHelper;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace SelfHosting
{
    class Service : WindowsServiceWCFHostManager
    {
        static void Main(string[] args)
        {
            if (Environment.UserInteractive)
            {
                Service service = new Service();
                service.Start();
                Console.WriteLine("Press Enter to quit");
                Console.ReadLine();
                service.Stop();
            }
            else
            {
                ServiceBase.Run(new Service());
            }
        }

        /// <summary>
        /// Name of the service when running as  a windows service
        /// </summary>
        public override string WindowsServiceName
        {
            get
            {
                return "Selfhosted WCF service";
            }
        }

        public override IEnumerable<System.ServiceModel.ServiceHost> RegisterServiceHosts()
        {
            List<ServiceHost> hosts = new List<ServiceHost>();
            hosts.Add(new ServiceHost(typeof(Service1)));
            return hosts;
        }
    }
}