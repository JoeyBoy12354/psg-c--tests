using System.Diagnostics;
using System.Net;
using System.Security.Principal;

namespace psg_automated_nunit_shared.Global
{
    public static class GlobalHostData
    {

        public static string Host => Dns.GetHostName();
        public static string Ip => GetHostIpAddress();
        public static string User => GetUser();


        private static string GetHostIpAddress()
        {
            // Get the host name

            // Get the IP addresses associated with the host name
            IPAddress[] addresses = Dns.GetHostAddresses(Host);

            // Find the first IPv4 address (assuming you want IPv4)
            foreach (IPAddress addr in addresses)
            {
                if (addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return addr.ToString();
                }
            }

            // If no IPv4 address found, return null or handle accordingly
            return "";
        }

        private static string GetUser()
        {
           // var userID = Environment.UserName;

            string userID = WindowsIdentity.GetCurrent().Name ?? "";

            return userID;
        }
    }
}
