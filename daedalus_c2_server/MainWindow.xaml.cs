using System;
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
        public MainWindow()
        {
            InitializeComponent();
            // All custom stuff here.
            botHandler = new BotHandler();
        }


       
    }
}
