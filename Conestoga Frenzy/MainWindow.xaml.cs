//File: MainWindow.xaml.cs
//Name: Steven Johnston, Matthew Warren
//Date: 11/18/2015
//Description: 
//      Starting point of project. Initiates Game
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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

namespace Conestoga_Frenzy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// <seealso cref="System.Windows.Window" />
    /// <seealso cref="System.Windows.Markup.IComponentConnector" />
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Canvas.SetRight(spHighScore,game.ActualWidth);
            lblIP.Content = GetLocalIPAddress();
            Game theGame = new Game(this);
            Thread gameThread = new Thread(theGame.start);
            gameThread.SetApartmentState(ApartmentState.STA);
            gameThread.Start();    
        }
        /// <summary>
        /// Gets the local ip address.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">Local IP Address Not Found!</exception>
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
            throw new Exception("Local IP Address Not Found!");
        }
    }
}
