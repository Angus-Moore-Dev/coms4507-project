using System.Net;
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
        public MainWindow()
        {
            InitializeComponent();
            // All custom stuff here.
            Initialise();

            botHandler = new BotHandler(ip);

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
    }
}
