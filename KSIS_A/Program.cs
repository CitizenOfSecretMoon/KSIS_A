using System;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace KSIS_A
{
    class Program
    {
        static void Main(string[] args)
        {
            bool flag = false;
            string Buf;
            string hostname;
            hostname = Console.ReadLine();
            int timeout = 1000;
            int max_ttl = 30;
            int current_ttl = 0;
            const int bufferSize = 32;
            byte[] buffer = new byte[bufferSize];
            new Random().NextBytes(buffer);
            Ping pinger = new Ping();
            Console.WriteLine($"Started ICMP Trace route on {hostname}");
            for (int ttl = 1; ttl <= max_ttl && !flag; ttl++)
            {
                current_ttl++;
                DateTime s1 = DateTime.Now;
                PingOptions options = new PingOptions(ttl, true);
                for (int j = 1; j <= 3; j++)
                {


                    PingReply reply = null;
                    try
                    {

                        reply = pinger.Send(hostname, timeout, buffer, options);

                    }
                    catch
                    {
                        Console.WriteLine("Error");

                        break;
                    }

                    if (reply != null) //dont need this
                    {
                        if (reply.Status == IPStatus.TtlExpired)
                        {
                            DateTime s3 = DateTime.Now;
                            Buf = TimeSpan.FromTicks((s3.Ticks) - (s1.Ticks)).TotalMilliseconds.ToString();
                            Buf = Buf.Substring(0, Buf.IndexOf(","));
                            Console.WriteLine($"[{ttl}] - Route: {reply.Address} - Time: { Buf} ms");

                            continue;
                        }
                        if (reply.Status == IPStatus.TimedOut)
                        {
                            //this would occour if it takes too long for the server to reply or if a server has the ICMP port closed (quite common for this).
                            Console.WriteLine($"Timeout on {hostname}. Continuing.");

                            continue;
                        }
                        if (reply.Status == IPStatus.Success)
                        {
                            DateTime s3 = DateTime.Now;
                            Buf = TimeSpan.FromTicks((s3.Ticks) - (s1.Ticks)).TotalMilliseconds.ToString();
                            Buf = Buf.Substring(0, Buf.IndexOf(","));
                            //the ICMP packet has reached the destination (the hostname)
                            Console.WriteLine($"Successful Trace route to {hostname} in {Buf} ms");
                            flag = true;
                        }
                    }
                }               
            }
        }
    }
}
