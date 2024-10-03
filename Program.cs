using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Thread monitorThread = new Thread(() => IpMonitor.StartMonitoring());
            monitorThread.Start();
        }
    }
}
