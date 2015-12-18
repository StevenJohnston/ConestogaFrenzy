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
    public partial class MainWindow : Window
    {
        public List<Player> players = new List<Player>();
        Stage stage = new Stage();
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
