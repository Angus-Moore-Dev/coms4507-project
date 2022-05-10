using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Coms4507_Project.BotHandling
{
    internal class NetworkHandler
    {
        private readonly UdpClient udpClient;
        private readonly int portNumber = 45920;
        private IPEndPoint endpoint;

        public NetworkHandler()
        {
            udpClient = new UdpClient(portNumber);
            endpoint = new IPEndPoint(IPAddress.Any, portNumber);
        }

        /// <summary>
        /// Send data back to the specified IP Address.
        /// </summary>
        /// <param name="message">the contents of the message to be sent back.</param>
        /// <param name="addr">the CIDR IP address (192.168.0.1)</param>
        public void SendMessage(string message, string addr)
        {
            IPEndPoint client = new IPEndPoint(new IPAddress(Encoding.UTF8.GetBytes(addr)), portNumber);
            byte[] dataB = Encoding.UTF8.GetBytes(message);
            _ = udpClient.Send(dataB, dataB.Length, client);
        }

        /// <summary>
        /// Wait for a message.
        /// </summary>
        /// <returns>String containing what the server received in a packet.</returns>
        public string WaitForMessage()
        {
            byte[] dataB = udpClient.Receive(ref endpoint); // This is blocking.
            string data = Encoding.UTF8.GetString(dataB);
            return data;
        }

    }


    /// <summary>
    /// The purpose of this is to contact the DNS server that we've written. It has information regarding all active bots and engages in the handoff that we use between 
    /// the bots and the C2 server.
    /// </summary>
    internal class NameserverContactHandler
    {
        private readonly HttpClient httpClient;

        public NameserverContactHandler()
        {
            httpClient = new HttpClient();
        }

        public string GetBotIPs()
        {
            return "";
        }

        public string GetServerStatus()
        {
            return "";
        }

    }
}
