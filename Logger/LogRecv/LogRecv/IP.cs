using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace LogRecv
{
   class IP
    {
        const string portSavePath = @"c:/LogRecv/Ip.config";

        public static string Pv4
        {
            get
            {
                return Dns.GetHostEntry(Dns.GetHostName())
                .AddressList.First(f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                .ToString();
            }
        }




        public static int Port
        {
            get
            {
                int value = 8765;
                if (File.Exists(portSavePath))
                {
                    var portStr = File.ReadAllText(portSavePath);

                    if (int.TryParse(portStr, out value))
                        return value;
                }
                return value;
            }
            set
            {
                var parent = Path.GetDirectoryName(portSavePath);
                if (!Directory.Exists(parent))
                    Directory.CreateDirectory(parent);

                File.WriteAllText(portSavePath, value.ToString());
            }
        }
    }
}
