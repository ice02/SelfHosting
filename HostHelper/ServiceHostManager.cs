/*
 * WCF Self hosting sample from www.WayneCliffordBarker.co.za
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;
using System.Text;
using System.Threading.Tasks;

namespace HostHelper
{
    public class ServiceHostManager
    {
        public List<ServiceHost> Hosts { get; set; }

        private string hostName;

        public ServiceHostManager(string HostName)
        {
            hostName = HostName;
            Hosts = new List<ServiceHost>();
        }

        public void OpenHosts()
        {
            List<ServiceHost> hostsToRemove = new List<ServiceHost>();
            bool failedToHost = false;

            // Iterate through the hosts and attempt to open them
            foreach (ServiceHost host in Hosts)
            {
                failedToHost = true;

                if (host != null)
                {
                    try
                    {
                        WriteLog(EventLogEntryType.Warning, string.Format("Starting to host service : '{0}'", host.Description.Name));
                        host.Open();
                        WriteLog(EventLogEntryType.SuccessAudit, string.Format("Service '{0}' hosted.", host.Description.Name));


                        StringBuilder hostBehaviourDetails = ReadHostBehaviours(host);
                        Console.WriteLine(hostBehaviourDetails.ToString());
                        Trace.TraceInformation(hostBehaviourDetails.ToString());

                        if (Environment.UserInteractive)
                        {
                            Trace.TraceInformation(string.Format("Namespace : {0}", host.Description.Namespace));

                            foreach (ServiceEndpoint endPoint in host.Description.Endpoints)
                            {
                                string address = endPoint.Address.ToString();

                                StringBuilder endpointDetails = new StringBuilder();

                                endpointDetails.AppendLine(endPoint.Name);
                                endpointDetails.AppendLine(string.Format("Address: {0}", address));
                                endpointDetails.AppendLine(string.Format("Binding: {0} ({1})", endPoint.Binding.Name, endPoint.Binding.Scheme));
                                endpointDetails.AppendLine(string.Format("Contract: {0}", endPoint.Contract.Name));

                                ReadBindingSecurityConfiguration(endPoint, endpointDetails);

                                Console.WriteLine(endpointDetails.ToString());
                                Trace.TraceInformation(endpointDetails.ToString());
                            }

                        }
                        failedToHost = false;
                    }
                    catch (ObjectDisposedException e)
                    {
                        WriteLog(EventLogEntryType.Error, string.Format("Failed to host service : '{0}' - '{1}' ", host.Description.Name, e.ToString()));
                    }
                    catch (System.ServiceProcess.TimeoutException e)
                    {
                        WriteLog(EventLogEntryType.Error, string.Format("Failed to host service : '{0}' - '{1}' ", host.Description.Name, e.ToString()));
                    }
                    catch (CommunicationObjectFaultedException e)
                    {
                        WriteLog(EventLogEntryType.Error, string.Format("Failed to host service : '{0}' - '{1}' ", host.Description.Name, e.ToString()));
                    }
                    catch (InvalidOperationException e)
                    {
                        WriteLog(EventLogEntryType.Error, string.Format("Failed to host service : '{0}' - '{1}' ", host.Description.Name, e.ToString()));
                    }
                }

                if (failedToHost)
                {
                    hostsToRemove.Add(host);
                }
                Console.WriteLine();
            }

            // Remove the hosts that did not open
            foreach (ServiceHost host in hostsToRemove)
            {
                Hosts.Remove(host);
                Debugger.Log(0, "WCF Hosts", string.Format("Unable to load : '{0}'", hostName));
            }

            if (hostsToRemove.Count == 0)
            {
                WriteLog(EventLogEntryType.SuccessAudit, string.Format("'{0}' Started.", hostName));
            }
        }

        private static StringBuilder ReadHostBehaviours(ServiceHost host)
        {
            StringBuilder hostBehaviourDetails = new StringBuilder();

            if (host.Description.Behaviors.Contains(typeof(ServiceBehaviorAttribute)))
            {
                ServiceBehaviorAttribute behaviour = host.Description.Behaviors[typeof(ServiceBehaviorAttribute)] as ServiceBehaviorAttribute;
                hostBehaviourDetails.AppendLine(string.Format("Behaviour: ServiceBehaviorAttribute"));
                hostBehaviourDetails.AppendLine(string.Format("Instance Context Mode: {0}", behaviour.InstanceContextMode.ToString()));
                hostBehaviourDetails.AppendLine(string.Format("Concurrency Mode: {0}", behaviour.ConcurrencyMode.ToString()));
                hostBehaviourDetails.AppendLine(string.Format("Transaction Isolation Level: {0}", behaviour.TransactionIsolationLevel.ToString()));
                hostBehaviourDetails.AppendLine(string.Format("Include ExceptionDetail In Faults: {0}", behaviour.IncludeExceptionDetailInFaults.ToString()));
                hostBehaviourDetails.AppendLine();
            }
            return hostBehaviourDetails;
        }

        private static void ReadBindingSecurityConfiguration(ServiceEndpoint endPoint, StringBuilder endpointDetails)
        {
            if (endPoint.Binding is NetTcpBinding)
            {
                NetTcpBinding binding = endPoint.Binding as NetTcpBinding;

                endpointDetails.AppendLine(string.Format("Binding Security Mode: {0}", binding.Security.Mode.ToString()));
                if (binding.Security.Mode != SecurityMode.None)
                {
                    endpointDetails.AppendLine(string.Format("Transport Credential Type: {0}", binding.Security.Transport.ClientCredentialType.ToString()));
                    endpointDetails.AppendLine(string.Format("Transport Protection Level: {0}", binding.Security.Transport.ProtectionLevel.ToString()));
                    endpointDetails.AppendLine(string.Format("Message Credential Type: {0}", binding.Security.Message.ClientCredentialType.ToString()));
                }
            }

            if (endPoint.Binding is NetHttpBinding)
            {
                NetHttpBinding binding = endPoint.Binding as NetHttpBinding;

                endpointDetails.AppendLine(string.Format("Binding Security Mode: {0}", binding.Security.Mode.ToString()));
                if (binding.Security.Mode != BasicHttpSecurityMode.None)
                {
                    endpointDetails.AppendLine(string.Format("Transport Credential Type: {0}", binding.Security.Transport.ClientCredentialType.ToString()));
                    endpointDetails.AppendLine(string.Format("Message Credential Type: {0}", binding.Security.Message.ClientCredentialType.ToString()));
                }
            }

            if (endPoint.Binding is NetHttpsBinding)
            {
                NetHttpsBinding binding = endPoint.Binding as NetHttpsBinding;

                endpointDetails.AppendLine(string.Format("Binding Security Mode: {0}", binding.Security.Mode.ToString()));
                endpointDetails.AppendLine(string.Format("Transport Credential Type: {0}", binding.Security.Transport.ClientCredentialType.ToString()));
                endpointDetails.AppendLine(string.Format("Message Credential Type: {0}", binding.Security.Message.ClientCredentialType.ToString()));

            }

            if (endPoint.Binding is NetMsmqBinding)
            {
                NetMsmqBinding binding = endPoint.Binding as NetMsmqBinding;

                endpointDetails.AppendLine(string.Format("Binding Security Mode: {0}", binding.Security.Mode.ToString()));
                if (binding.Security.Mode != NetMsmqSecurityMode.None)
                {
                    endpointDetails.AppendLine(string.Format("Transport Credential Type: {0}", binding.Security.Transport.MsmqAuthenticationMode.ToString()));
                    endpointDetails.AppendLine(string.Format("Transport Protection Level: {0}", binding.Security.Transport.MsmqProtectionLevel.ToString()));
                    endpointDetails.AppendLine(string.Format("Message Credential Type: {0}", binding.Security.Message.ClientCredentialType.ToString()));
                }
            }
        }

        public void CloseHosts()
        {
            //WriteToEventLog(EventLogEntryType.Warning, string.Format("Closing host: '{0}'", hostName));

            foreach (ServiceHost host in Hosts)
            {
                if (host != null)
                {
                    try
                    {
                        host.Close();
                    }
                    catch (ObjectDisposedException e)
                    {
                        WriteLog(EventLogEntryType.Error, string.Format("Failed to close service : '{0}' - '{1}' ", host.Description.Name, e.ToString()));
                        throw;
                    }
                    catch (System.ServiceProcess.TimeoutException e)
                    {
                        WriteLog(EventLogEntryType.Error, string.Format("Failed to close service : '{0}' - '{1}' ", host.Description.Name, e.ToString()));
                        throw;
                    }
                    catch (CommunicationObjectFaultedException e)
                    {
                        WriteLog(EventLogEntryType.Error, string.Format("Failed to close service : '{0}' - '{1}' ", host.Description.Name, e.ToString()));
                        throw;
                    }
                    catch (InvalidOperationException e)
                    {
                        WriteLog(EventLogEntryType.Error, string.Format("Failed to close service : '{0}' - '{1}' ", host.Description.Name, e.ToString()));
                        throw;
                    }
                }
            }
        }

        private void WriteLog(EventLogEntryType logType, string logEntry)
        {
            Trace.TraceInformation(logEntry);
            try
            {
                EventLog log = new EventLog();
                log.Source = hostName;
                log.WriteEntry(logEntry, logType);
            }
            catch (Exception)
            {
                // Swallow the exception.   
            }

            switch (logType)
            {
                case EventLogEntryType.Warning:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;

                case EventLogEntryType.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;

                case EventLogEntryType.SuccessAudit:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;

                default:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }

            Console.WriteLine(string.Format("{0} - {1}", logType.ToString(), logEntry));
            Console.ForegroundColor = ConsoleColor.White;
        }

    }
}
