//File: Player.cs
//Name: Steven Johnston, Matthew Warren
//Date: 11/18/2015
//Description: 
//      This class contains functionality for a player.
//      A player is a successfull new connection to the game. 
//      Player holds drawing and updating.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Conestoga_Frenzy
{
    public class Player
    {
        /// <summary>
        /// Players current Sprite
        /// </summary>
        int SpriteCounter = 0;
        /// <summary>
        /// Speed of the sprite image change
        /// </summary>
        const int SPRITE_SPEED = 12;

        /// <summary>
        /// The max_speed
        /// </summary>
        const double MAX_SPEED = 0.8;
        /// <summary>
        /// This is acceleration
        /// </summary>
        const double MAX_SPEED_CHANGE = 0.10;
        /// <summary>
        /// Not used will be used soon
        /// </summary>
        const double MAX_SPEED_ON_X = 2;
        /// <summary>
        /// How fast to being to accelerate
        /// </summary>
        const double SPEED_DIVISOR = 10;
        /// <summary>
        /// The weight of the player. high makes them bounce more
        /// </summary>
        const double WEIGHT = 2;
        /// <summary>
        /// how long it takes to accelerate
        /// </summary>
        const double SPEED_POWER = 9;

        /// <summary>
        /// The size of the player ratio
        /// </summary>
        const double SIZE_RATIO = 2;

        /// <summary>
        /// The identifier
        /// </summary>
        public long id;
        /// <summary>
        /// The colour
        /// </summary>
        public Color colour;
        /// <summary>
        /// The position on the screen
        /// </summary>
        public Point position;
        /// <summary>
        /// The size of the player
        /// </summary>
        public Point size;
        /// <summary>
        /// The velocity
        /// </summary>
        public Point velocity;
        /// <summary>
        /// The acceleration
        /// </summary>
        public Point acceleration;
        /// <summary>
        /// The speed on the x axis for equation
        /// </summary>
        public Point speedOnX;
        /// <summary>
        /// The score
        /// </summary>
        public int score =0;

        /// <summary>
        /// Index of image on canvas
        /// </summary>
        int canvasItemIndex = -1;
        /// <summary>
        /// This image element on canvas
        /// </summary>
        Image thisElement;

        /// <summary>
        /// Is this player alive
        /// </summary>
        public bool isAlive;
        /// <summary>
        /// Player velocity after the collide with player(s)
        /// </summary>
        public Point afterCollisionVelocity;
        /// <summary>
        /// The current image in sprite
        /// </summary>
        public BitmapSource currentImage;
        /// <summary>
        /// The sprite
        /// </summary>
        public BitmapSource sprite;
        /// <summary>
        /// Current postion in sprite
        /// </summary>
        public int spriteIndex = 0;
        /// <summary>
        /// Number of sprite frames
        /// </summary>
        int spriteFrames = 8;

        /// <summary>
        /// Set true if user has disconnected
        /// </summary>
        public bool killMe = false;
        /// <summary>
        /// Stop watch for killing 
        /// </summary>
        Stopwatch stopWatch = new Stopwatch();

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        /// <param name="newId">The new identifier.</param>
        /// <param name="newColour">The new colour.</param>
        public Player(long newId, Color newColour)
        {
            id = newId;
            colour = newColour;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        /// <param name="newId">The new identifier.</param>
        /// <param name="newColour">The new colour.</param>
        public Player(long newId, string newColour)
        {
            id = newId;
            colour = (Color)ColorConverter.ConvertFromString(newColour);//String to colour
            size = new Point(100, 100);
            isAlive = false;
            velocity = new Point(0, 0);
            position = new Point(100, 100);
            System.Drawing.Bitmap tempMap = Properties.Resources.ballSprite;

            stopWatch.Start();
            sprite = ExtBitmap.ToBitmapImage(ExtBitmap.ColorTint(tempMap, colour.B, colour.G, colour.R));
            
            sprite.Freeze();
        }
        /// <summary>
        /// Moves this instance. Increase the players postion by its velocity
        /// </summary>
        public void move()
        {
            if(stopWatch.ElapsedMilliseconds > 2000)
            {
                killMe = true;
                isAlive = false;
            }
            if (isAlive)
            {
                //used to prevent accelerating to fast
                Point changeInAcceleration = new Point(acceleration.X - velocity.X, acceleration.Y - velocity.Y);


                if (changeInAcceleration.X > MAX_SPEED_CHANGE)
                {
                    changeInAcceleration.X = MAX_SPEED_CHANGE;
                }
                if (changeInAcceleration.X < -MAX_SPEED_CHANGE)
                {
                    changeInAcceleration.X = -MAX_SPEED_CHANGE;
                }
                if (changeInAcceleration.Y > MAX_SPEED_CHANGE)
                {
                    changeInAcceleration.Y = MAX_SPEED_CHANGE;
                }
                if (changeInAcceleration.Y < -MAX_SPEED_CHANGE)
                {
                    changeInAcceleration.Y = -MAX_SPEED_CHANGE;
                }
                //Change speed
                speedOnX.X += changeInAcceleration.X;
                speedOnX.Y += changeInAcceleration.Y;

                //keep speed in range
                if (speedOnX.X > MAX_SPEED)
                {
                    speedOnX.X = MAX_SPEED;
                }
                if (speedOnX.X < MAX_SPEED * -1)
                {
                    speedOnX.X = MAX_SPEED * -1;
                }
                if (speedOnX.Y > MAX_SPEED)
                {
                    speedOnX.Y = MAX_SPEED;
                }
                if (speedOnX.Y < MAX_SPEED * -1)
                {
                    speedOnX.Y = MAX_SPEED * -1;
                }

                //update velocity
                velocity.X += Math.Pow(speedOnX.X, SPEED_POWER) / SPEED_DIVISOR;
                velocity.Y += Math.Pow(speedOnX.Y, SPEED_POWER) / SPEED_DIVISOR;

                int polarity = 0;
                polarity = velocity.X > 0 ? 1 : -1;
                position.X += velocity.X;//Math.Pow(velocity.X,1) * polarity;
                polarity = velocity.Y > 0 ? 1 : -1;
                position.Y += velocity.Y;//Math.Pow(velocity.Y,1) * polarity;
            }
        }
        /// <summary>
        /// Checks if player collides with any other player
        /// </summary>
        /// <param name="players">The players.</param>
        public void collision(List<Player> players)
        {

            afterCollisionVelocity = new Point(velocity.X,velocity.Y);
            foreach (Player player in players)
            {
                if (player.id != this.id)//this != player might work
                {
                    if (collides(player))
                    {
                        //Velocity when hit
                        afterCollisionVelocity.X += (player.velocity.X * Math.Abs(player.acceleration.X / 2))- (plusOrMinus(afterCollisionVelocity.X) * WEIGHT);
                        afterCollisionVelocity.Y += (player.velocity.Y * Math.Abs(player.acceleration.Y / 2)) - (plusOrMinus(afterCollisionVelocity.Y) * WEIGHT);
                    }
                }
            }
        }
        /// <summary>
        /// Updates the acceleration based of tilt from phone.
        /// </summary>
        /// <param name="tilt">The tilt.</param>
        public void updateAcceleration(Point tilt)
        {
            stopWatch.Reset();
            stopWatch.Start();
            if (isAlive)
            {
                acceleration.X = tilt.X * -1;
                acceleration.Y = tilt.Y;
            }
        }
        /// <summary>
        /// Updates the velocity to after collion velocity.
        /// </summary>
        public void updateVelocity()
        {
            velocity = afterCollisionVelocity;
        }
        /// <summary>
        /// Checks if collision with another player.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns></returns>
        public bool collides(Player player)
        {
            bool doesCollide = false;
            double maxDistance = size.X / 2 + player.size.X / 2;
            if (Hypotenuse(position.X-player.position.X,position.Y-player.position.Y) <= maxDistance)
            {
                doesCollide = true;
            }
            return doesCollide;
        }

        /// <summary>
        /// Checks if player has fallen of the stage
        /// </summary>
        /// <param name="myCanvas">My canvas.</param>
        /// <param name="stage">The stage.</param>
        public void OnPlat(MyCanvas myCanvas, Stage stage)
        {
            if (isAlive)
            {
                Point centerOfScreen = new Point(myCanvas.ActualWidth / 2, myCanvas.ActualHeight / 2);
                double DistanceFromCenter = Hypotenuse(centerOfScreen.X - position.X, centerOfScreen.Y - position.Y);

                //Checks if fell off stage
                if (DistanceFromCenter > stage.maxFromCenter + size.X / 4)
                {
                    isAlive = false;
                    velocity = new Point(0, 0);
                    acceleration = new Point(0, 0);
                    speedOnX = new Point(0, 0);
                    //Console.WriteLine("Die");
                }
            }
        }


        /// <summary>
        /// changes image to next sprite
        /// </summary>
        public void nextSprite()
        {
            if (isAlive)
            {   
                SpriteCounter++;
                if (SpriteCounter > SPRITE_SPEED)
                {
                    spriteIndex++;
                    if (spriteIndex == spriteFrames)
                    {
                        spriteIndex = 0;
                    }
                    currentImage = new CroppedBitmap(sprite, new Int32Rect((int)(sprite.PixelWidth / spriteFrames) * spriteIndex, 0, (int)(sprite.PixelWidth / spriteFrames), (int)(sprite.PixelWidth / spriteFrames)));

                    thisElement.Source = currentImage;
                    SpriteCounter = 0;
                }
            }
            //currentImage = ToBitmapImage(ExtBitmap.ColorTint(BitmapFromSource(tempImage), colour.B,colour.G,colour.R));
        }


        /// <summary>
        /// Draws the specified canvas.
        /// </summary>
        /// <param name="myCanvas">My canvas.</param>
        public void Draw(MyCanvas myCanvas)
        {
            if (!killMe)
            {
                nextSprite();
                if (canvasItemIndex == -1)
                {
                    this.thisElement = new Image();
                    thisElement.Width = size.X;
                    thisElement.Height = size.Y;
                }
                //img.Source = currentImage;
                //img.Source = player.currentImage;
                Canvas.SetLeft(thisElement, position.X - thisElement.Width / 2);
                Canvas.SetTop(thisElement, position.Y - thisElement.Width / 2);
                if (canvasItemIndex == -1)
                {
                    canvasItemIndex = myCanvas.Children.Add(thisElement);
                }
            }
            else
            {
                if (canvasItemIndex != -1)
                {
                    myCanvas.Children.Remove(thisElement);
                    canvasItemIndex = -1;
                }
            }
        }
        /// <summary>
        /// Get hypotenuse from 2 sides.
        /// </summary>
        /// <param name="side1">The side1.</param>
        /// <param name="side2">The side2.</param>
        /// <returns></returns>
        static double Hypotenuse(double side1, double side2)
        {
            return Math.Sqrt(side1 * side1 + side2 * side2);
        }
        /// <summary>
        /// Pluses the or minus.
        /// </summary>
        /// <param name="num">The number.</param>
        /// <returns></returns>
        int plusOrMinus(double num)
        { 
            int returner = 1;
            if(num<0) {
                returner = -1;
            }
            return returner;
        }

    }
}
