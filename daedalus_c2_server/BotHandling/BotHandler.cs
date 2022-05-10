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
        public BotHandler()
        {
            bots = new List<string>();
            networkHandler = new NetworkHandler();
            Thread thread = new Thread(Listener);
            thread.Start();
        }

        private void LoadBotIDs()
        {
            // TODO: Take the filePath containing all the bots, load them in and check for status on them (with their last known associated IP address).
        }
        private void Listener()
        {
            while(true)
            {
                try
                {
                    // We just loop over this over and over and wait for a message
                    Trace.WriteLine("waiting");
                    string message = networkHandler.WaitForMessage();
                    JObject jData = JsonConvert.DeserializeObject<JObject>(message);
                    
                    // All bots provide this information.
                    string ip = jData.GetValue("ip").ToString();
                    string hostID = jData.GetValue("id").ToString();
                    string status = jData.GetValue("status").ToString();
                   
                }
                catch (Exception)
                {
                    // We should just ignore it.
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
