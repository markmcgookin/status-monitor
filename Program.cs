using System;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;

namespace status_monitor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting");

            var path = "info.dat";
            if (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("STATUS_OUTPUT")))
            {
                path = Environment.GetEnvironmentVariable("STATUS_OUTPUT");
            }

            Console.WriteLine("Output: " + path);

            while (true)
            {
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    throw new NotSupportedException("This only works for linux at the minute as I can't be bothered making it cross platform");
                }

                var ip = IPAddress.None;
                try
                {
                    using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                    {
                        // Connect socket to Google's Public DNS service
                        socket.Connect("8.8.8.8", 65530);
                        if (!(socket.LocalEndPoint is IPEndPoint endPoint))
                        {
                            throw new InvalidOperationException($"Error occurred casting {socket.LocalEndPoint} to IPEndPoint");
                        }
                        ip = endPoint.Address;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"An error occcured {e.Message}");
                }

                try
                {
                    var cpu_bash = $"top -bn1 | grep load | awk '{{printf \"%.2f\", $(NF-2)}}'";
                    var cpu = Bash(cpu_bash);

                    var mem_bash = $"free -m | awk 'NR==2{{printf \"%s/%sMB %.2f%%\", $3,$2,$3*100/$2 }}'";
                    var mem = Bash(mem_bash);

                    var disk_bash = "df -h | awk '$NF==\"/\"{printf \"%d/%dGB %s\", $3,$2,$5}'";
                    var disk = Bash(disk_bash);

                    var date = System.DateTime.Now.ToString("dd-MM-yyyy");
                    var time = System.DateTime.Now.ToString("HH:mm:ss");

                    Console.WriteLine($"{ip}\t{cpu}%\t{mem}\t{disk}\t{date}\t{time}");
                    File.WriteAllText(path, $"{ip}\t{cpu}%\t{mem}\t{disk}\t{date}\t{time}", Encoding.UTF8);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"An exception occcured {e.Message}");
                }
            }
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
