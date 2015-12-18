//File: Game.cs
//Name: Steven Johnston, Matthew Warren
//Date: 11/18/2015
//Description: 
//      The game class that handles the game flow. Handles game thread (update/draw)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Conestoga_Frenzy
{
    /// <summary>
    /// 
    /// </summary>
    public class Game
    {
        /// <summary>
        /// The lock for players list
        /// </summary>
        object _playersLock = new object();
        /// <summary>
        /// The canvas size
        /// </summary>
        Point CanvasSize;
        /// <summary>
        /// The draw timer
        /// </summary>
        MyTimer drawTimer;
        /// <summary>
        /// The update timer
        /// </summary>
        MyTimer updateTimer;
        /// <summary>
        /// The socket recive thread
        /// </summary>
        Thread socketRecive;

        /// <summary>
        /// The title text
        /// </summary>
        string titleText = "Game Starting";

        /// <summary>
        /// The high scores
        /// </summary>
        HighScore highScores;

        /// <summary>
        /// The players
        /// </summary>
        public List<Player> players = new List<Player>();
        /// <summary>
        /// The stage
        /// </summary>
        Stage stage = new Stage();

        /// <summary>
        /// The game canvas
        /// </summary>
        MyCanvas game;
        /// <summary>
        /// The window 
        /// </summary>
        MainWindow window;
        /// <summary>
        /// Initializes a new instance of the <see cref="Game"/> class.
        /// </summary>
        /// <param name="myWindow">My window.</param>
        public Game(MainWindow myWindow)
        {

            game = myWindow.game;
            window = myWindow;
        }
        /// <summary>
        /// Starts the game
        /// </summary>
        public void start()
        {

            game.Dispatcher.Invoke((Action)(() =>
            {
                CanvasSize = new Point(game.ActualWidth, game.ActualHeight);
            }));

            highScores = new HighScore(window.txtScores);
            socketRecive = new Thread(new ThreadStart(udpRevice));
            socketRecive.SetApartmentState(ApartmentState.STA);
            socketRecive.Start();
            updateTimer = new MyTimer(1000 / 120, update, new object());
            drawTimer = new MyTimer(1000 / 60, draw, game);
            drawTimer.start();
            Thread.Sleep(500);
            //drawTimer.pause();
            updateTimer.start();
            updateTimer.pause();
            for (;;)
            {
                //Wait for at least two players
                for (; players.Count < 1;)
                {
                    Console.WriteLine("Players connected: " + (players.Count));
                    titleText = "Play Me";
                    Thread.Sleep(100);
                }
                if (players.Count > 1)
                {
                    for (int i = 10; i > 0; i--)
                    {

                        Console.WriteLine("Game starting: " + i);
                        titleText = "Game starting: " + i;
                        Thread.Sleep(333);
                    }
                }

                StartGame();

                //if(!drawTimer.thread.IsAlive)drawTimer.start();
                //drawTimer.resume();
                Thread.Sleep(3000);
                updateTimer.resume();
                //if(!updateTimer.thread.IsAlive)updateTimer.start();
                //Wait for all but on player to die
                for (; players.Count(x => x.isAlive) > 1;)
                {
                    Thread.Sleep(100);
                }
                game.Dispatcher.Invoke((Action)(() =>
                {
                    Player winner = players.Find(x=> x.isAlive);
                    if (winner != null && players.Count > 1)
                    {
                        titleText = "Winner";
                        winner.score++;
                        highScores.updatePlayer(winner);
                    }
                    else if(players.Count >1){
                        titleText = "Tie Game";
                    }
                }));
                Thread.Sleep(1000);
                stage.Reset();

                //drawTimer.pause();
                updateTimer.pause();
            }
        }
        /// <summary>
        /// Recives udp packets
        /// </summary>
        public void udpRevice()
        {

            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 27940);
            UdpClient newsock = new UdpClient(ipep);
            while (true)
            {
                try
                {

                    byte[] data = new byte[256];
                    //Console.WriteLine("Waiting for a client...");
                    IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                    data = newsock.Receive(ref sender);

                    //Console.WriteLine("Message received from {0}:", sender.ToString());
                    //Console.WriteLine(Encoding.ASCII.GetString(data, 0, data.Length));
                    string stringData = System.Text.Encoding.Default.GetString(data);
                    string[] datas = stringData.Split('|');
                    long senderId = Convert.ToInt64(datas[0]);
                    if (datas.Count() == 2 && !players.Exists(x => x.id == senderId))
                    {
                        string colour = datas[1];
                        Player newPlayer = new Player(senderId, colour);
                        lock(_playersLock)
                        {
                            players.Add(newPlayer);
                        }
                    }
                    else if (datas.Count() == 3)
                    {
                        Player thisPlayer = players.Find(x => x.id == senderId);
                        if (thisPlayer != null)
                        {
                            Point tilt = new Point(Convert.ToDouble(datas[1]), Convert.ToDouble(datas[2]));
                            thisPlayer.updateAcceleration(tilt);
                            Console.WriteLine(tilt.X + " " + tilt.Y);
                        }
                        //Change velocity
                    }
                }
                catch (Exception ex)
                {
                    // log errors
                }
            }
        }
        /// <summary>
        /// Updates game
        /// </summary>
        /// <param name="obj">The object.</param>
        public void update(object obj)
        {
            game.Dispatcher.Invoke((Action)(() =>
            {
                //Move all
                lock(_playersLock)
                {
                    players.ForEach(x => x.move());
                }
                //Check collisions 
                lock(_playersLock)
                {
                    players.ForEach(x => x.collision(players));
                }
                //And make new postion for each 
                lock(_playersLock)
                {
                    players.ForEach(x => x.updateVelocity());
                }
                //Check if dead
                lock(_playersLock)
                {
                    players.ForEach(x => x.OnPlat(game, stage));
                }
            }));
            stage.Update();

        }
        /// <summary>
        /// Draws the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        public void draw(object obj)
        {
            game.Dispatcher.Invoke((Action)(() =>
            {
                //MyCanvas game = obj as MyCanvas;
                //game.Children.Clear();
                game.DrawStage(stage);
                game.DrawPlayers(players);
                window.txtTitle.Content = titleText;
            }));
        }

        /// <summary>
        /// Starts the game.
        /// </summary>
        public void StartGame()
        {
            lock(_playersLock)
            {
                players.RemoveAll(x => x.killMe);
            }
            double angle = (2 * Math.PI) / players.Count;
            int count = 0;
            foreach (Player player in players)
            {
                count++;
                double oposite = Math.Sin(angle * count) * (stage.maxSize / 2) * stage.SPAWN_DISTANCE_RATIO;
                double adjacent = Math.Cos(angle * count) * (stage.maxSize / 2) * stage.SPAWN_DISTANCE_RATIO;
                player.position = new Point(CanvasSize.X / 2 + oposite, CanvasSize.Y / 2 + adjacent);
            }
            players.ForEach(x => x.isAlive = true);

        }
    }
}
