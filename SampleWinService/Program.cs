using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Text;
using System.Reflection;

namespace JITMessageProcessingWindowsService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
#if DEBUG
            JITMessageProcessingWindowsService myseervice = new JITMessageProcessingWindowsService();
            myseervice.OnDebug();
            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
#else
            //Make self-installing service
            if (args.Length > 0)
            {
                //Install service
                if (args[0].Trim().ToLower() == "/i")
                {
                    System.Configuration.Install.ManagedInstallerClass.InstallHelper(new string[] { "/i", Assembly.GetExecutingAssembly().Location });
                }
                //Uninstall service                 
                else if (args[0].Trim().ToLower() == "/u")
                {
                    System.Configuration.Install.ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
                }
            }
            else
            {
                System.ServiceProcess.ServiceBase[] ServicesToRun;
                ServicesToRun = new System.ServiceProcess.ServiceBase[] { new JITMessageProcessingWindowsService() };
                System.ServiceProcess.ServiceBase.Run(ServicesToRun);
            }
#endif

        }
    }
}
