using System;
using System.Management;
using System.Diagnostics;

namespace DisplaylinkLaggyFix
{
    class Program
    {
        static ProcessPriorityClass highPriority = ProcessPriorityClass.High;
        static string DisplayLinkProcessName = "WUDFHost";
        static string DisplayLinkDLLname = "dlidusb";
        static void FindAndSetPriority()
        {
            Process[] procs = Process.GetProcessesByName(DisplayLinkProcessName);
            foreach (var proc in procs)
            {
                foreach (var module in proc.Modules)
                {
                    if (module.ToString().Contains(DisplayLinkDLLname))
                    {
                        Console.WriteLine("DisplayLink process has been found. Set Priority to high.");
                        proc.PriorityClass = highPriority;
                        return;
                    }
                }
            }
        }
        static void HandleEvent(object sender, EventArrivedEventArgs e)
        {
            uint pid = (uint)e.NewEvent.Properties["ProcessID"].Value;
            Console.WriteLine("["+ DisplayLinkProcessName + ".exe] Found. PID=" + pid.ToString());
            Process dlProcess = Process.GetProcessById((int)pid);
            dlProcess.PriorityClass = highPriority;

            foreach (var module in dlProcess.Modules)
            {
                if (module.ToString().Contains(DisplayLinkDLLname))
                {
                    Console.WriteLine(pid.ToString() + " is a DisplayLink process. Priority has been set to high.\r\n");
                    dlProcess.PriorityClass = highPriority;
                    return;
                }
            }
            Console.WriteLine("It's not a DisplayLink process. Ignored.");
            return;
        }

        static void Main(string[] args)
        {
            if (Environment.Is64BitOperatingSystem && !Environment.Is64BitProcess)
            {
                Console.WriteLine("Please run the 64bit version of this program on 64bit OS.");
                Environment.Exit(1);
            }

            Console.WriteLine("If you can see this window, please use [Task Scheduler] to run this program(when computer starts), and set the runner user as [SYSTEM].\r\n\r\n");
            if (args.Length == 0)
            {
                Console.WriteLine("No arguments, use [WUDFHost] as DisplayLink process name.");
                Console.WriteLine("Use [dlidusb] as dll name.");
            }
            else if (args.Length == 1)
            {
                if (args[0].Contains(".exe"))
                {
                    Console.WriteLine("Please don't put [.exe] in the argument. Just put the name, like [WUDFHost].");
                    Environment.Exit(1);
                }
                Console.WriteLine("Use [" + args[0] + "] as DisplayLink process name.");
                Console.WriteLine("Use [dlidusb] as dll name.");
                DisplayLinkProcessName = args[0];
            }
            else if (args.Length == 2)
            {
                if (args[0].Contains(".exe"))
                {
                    Console.WriteLine("Please don't put [.exe] in the argument. Just put the name, like [WUDFHost].");
                    Environment.Exit(1);
                }
                Console.WriteLine("Use [" + args[0] + "] as DisplayLink process name.");
                Console.WriteLine("Use [" + args[1] + "] as dll name.");
                DisplayLinkProcessName = args[0];
                DisplayLinkDLLname = args[1];
            }
            else
            {
                Console.WriteLine("Too many arguments.");
                Environment.Exit(1);
            }

            FindAndSetPriority();
            Console.WriteLine(" ");
            Console.WriteLine("Initializing WMI...");


            ManagementEventWatcher watcher = new ManagementEventWatcher();
            WqlEventQuery query = new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace WHERE ProcessName='"+ DisplayLinkProcessName + ".exe'");
            watcher.EventArrived += new EventArrivedEventHandler(HandleEvent);
            watcher.Query = query;
            watcher.Start();

            while (true)
            {
                Console.WriteLine("Listening for new ["+ DisplayLinkProcessName + ".exe]");
                watcher.WaitForNextEvent();

            }
        }
    }
}
