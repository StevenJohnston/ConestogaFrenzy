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
    public class Game
    {
        object _playersLock = new object();
        Point CanvasSize;
        MyTimer drawTimer;
        MyTimer updateTimer;
        Thread socketRecive;

        string titleText = "Game Starting";

        HighScore highScores;

        public List<Player> players = new List<Player>();
        Stage stage = new Stage();

        MyCanvas game;
        MainWindow window;
        public Game(MainWindow myWindow)
        {

            game = myWindow.game;
            window = myWindow;
        }
        public void start()
        {

            game.Dispatcher.Invoke((Action)(() =>
            {
                CanvasSize = new Point(game.ActualWidth, game.ActualHeight);
            }));
            highScores = new HighScore(window.txtScores);
            socketRecive = new Thread(new ThreadStart(WorkThreadFunction));
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
                    else {
                        titleText = "Tie Game";
                    }
                }));
                Thread.Sleep(1000);
                stage.Reset();

                //drawTimer.pause();
                updateTimer.pause();
            }
        }
        public void WorkThreadFunction()
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
