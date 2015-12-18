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
        Point CanvasSize;
        MyTimer drawTimer;
        MyTimer updateTimer;

        Thread socketRecive; 

        public List<Player> players = new List<Player>();
        Stage stage = new Stage();
        public MainWindow()
        {
            InitializeComponent();
            Game theGame = new Game(game);
            Thread gameThread = new Thread(theGame.start);
            gameThread.Start();

            //this.Loaded += new RoutedEventHandler(Game);
                
        }
        
        
    }
}
