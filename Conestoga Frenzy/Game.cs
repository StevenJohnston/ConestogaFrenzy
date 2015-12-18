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
        Point CanvasSize;
        MyTimer drawTimer;
        MyTimer updateTimer;

        Thread socketRecive;

        public List<Player> players = new List<Player>();
        Stage stage = new Stage();

        MyCanvas game;
        public Game(MyCanvas myCanvas)
        {
            game = myCanvas;
        }
        public void start()
        {

            game.Dispatcher.Invoke((Action)(() =>
            {
                CanvasSize = new Point(game.ActualWidth, game.ActualHeight);
            }));
            socketRecive = new Thread(new ThreadStart(WorkThreadFunction));
            socketRecive.Start();
            updateTimer = new MyTimer(1000 / 120, update, new object());
            drawTimer = new MyTimer(1000 / 60, draw, game);
            drawTimer.start();
            drawTimer.pause();
            updateTimer.start();
            updateTimer.pause();
            for (; ; )
            {
                //Wait for at least two players
                for (; players.Count < 1; )
                {
                    Console.WriteLine("Players connected: " + (players.Count));
                    Thread.Sleep(100);
                }
                for (int i = 1; i > 0; i--)
                {
                    Console.WriteLine("Game starting: " + i);
                    Thread.Sleep(1000);
                }

                StartGame();

                //if(!drawTimer.thread.IsAlive)drawTimer.start();
                drawTimer.resume();
                Thread.Sleep(3000);
                updateTimer.resume();
                //if(!updateTimer.thread.IsAlive)updateTimer.start();
                //Wait for all but on player to die
                for (; players.Count(x => x.isAlive) > 1; )
                {
                    Thread.Sleep(100);
                }

                drawTimer.pause();
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
                        players.Add(newPlayer);
                    }
                    else if (datas.Count() == 3)
                    {
                        Player thisPlayer = players.Find(x => x.id == senderId);
                        Point tilt = new Point(Convert.ToDouble(datas[1]), Convert.ToDouble(datas[2]));
                        thisPlayer.updateAcceleration(tilt);
                        Console.WriteLine(tilt.X + " " + tilt.Y);
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
                players.ForEach(x => x.move());
                //Check collisions 
                players.ForEach(x => x.collision(players));
                //And make new postion for each 
                players.ForEach(x => x.updateVelocity());
                //Check if dead
                players.ForEach(x => x.OnPlat(game, stage));
            }));
            stage.Update();

        }
        public void draw(object obj)
        {
            game.Dispatcher.Invoke((Action)(() =>
            {
                //MyCanvas game = obj as MyCanvas;

                game.Children.Clear();
                game.DrawStage(stage);
                game.DrawPlayers(players);

            }));
        }

        public void StartGame()
        {
            double angle = (2 * Math.PI) / players.Count;
            int count = 0;
            foreach (Player player in players)
            {
                count++;
                double oposite = Math.Sin(angle * count) * (stage.maxSize / 2) * stage.SPAWN_DISTANCE_RATIO;
                double adjacent = Math.Sin((Math.PI / 4 - angle) * count) * (stage.maxSize / 2) * stage.SPAWN_DISTANCE_RATIO;
                player.position = new Point(CanvasSize.X / 2 + oposite, CanvasSize.Y / 2 + adjacent);
            }
            players.ForEach(x => x.isAlive = true);

        }
    }
}
