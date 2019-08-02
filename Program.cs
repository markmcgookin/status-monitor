using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace status_monitor
{
    class Program
    {
        static void Main(string[] args)
        {

            var host = Dns.GetHostEntry(Dns.GetHostName());
            var ip = host.AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
            var cpu_bash = "top -bn1 | grep load | awk '{printf \"%.2f\", $(NF-2)}'";
            if(RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                cpu_bash = "top -l 1 -n 1 | grep Load | awk '{printf \"%.2f\", $(NF-2)}'";
            }
            var cpu = Bash(cpu_bash);

            var mem_bash = "free -m | awk 'NR==2{printf \"%s/%sMB %.2f%%\", $3,$2,$3*100/$2 }'";
            if(RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                mem_bash = "top -l 1 -n 1 | grep Load | awk '{printf \"%.2f\", $(NF-2)}'";
            }

            var mem = Bash(mem_bash);

            Console.WriteLine(ip.ToString());
            Console.WriteLine(cpu.ToString());
            Console.WriteLine(mem.ToString());
        }

        public static string Bash(string cmd)
        {
            var escapedArgs = cmd.Replace("\"", "\\\"");
            
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return result;
        }
    }
}
