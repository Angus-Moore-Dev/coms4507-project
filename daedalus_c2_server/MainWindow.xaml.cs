﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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

            botHandler = new BotHandler();

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
