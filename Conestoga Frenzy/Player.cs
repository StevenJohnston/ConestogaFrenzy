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
        const int SPRITE_SPEED = 12;
        int SpriteCounter = 0;
        const double MAX_SPEED = 1;
        const double MAX_SPEED_CHANGE = 0.1;
        const double MAX_SPEED_ON_X = 1000000;
        const double SPEED_DIVISOR = 10;
        const double SIZE_RATIO = 2;
        public long id;
        public Color colour;
        public Point position;
        public Point size;
        public Point velocity;
        public Point acceleration;
        public Point speedOnX;

        int canvasItemIndex = -1;
        Image thisElement;

        public bool isAlive;
        public Point afterCollisionVelocity;
        //public BitmapImage currentImage;
        public BitmapSource currentImage;
        public BitmapSource sprite;
        public int spriteIndex = 0;
        int spriteFrames = 8;

        public bool killMe = false;
        Stopwatch stopWatch = new Stopwatch();

        public Player(long newId, Color newColour)
        {
            id = newId;
            colour = newColour;
        }
        public Player(long newId, string newColour)
        {
            id = newId;
            colour = (Color)ColorConverter.ConvertFromString(newColour);
            size = new Point(100, 100);
            isAlive = false;
            velocity = new Point(0, 0);
            position = new Point(100, 100);
            System.Drawing.Bitmap tempMap = Properties.Resources.ballSprite;

            stopWatch.Start();
            //BitmapSource tempImage = new CroppedBitmap(sprite, new Int32Rect((int)(sprite.PixelWidth / spriteFrames) * spriteIndex, 0, (int)(sprite.PixelWidth / spriteFrames), (int)(sprite.PixelWidth / spriteFrames)));
            sprite = ExtBitmap.ToBitmapImage(ExtBitmap.ColorTint(tempMap, colour.B, colour.G, colour.R));


            //sprite = ToBitmapImage(tempMap);
            sprite.Freeze();

            
            //sprite = new BitmapImage(new Uri("pack://application:,,,/Conestoga Frenzy;component/Resources/ballSprite.png", UriKind.Absolute));
        }
        public void move()
        {
            if(stopWatch.ElapsedMilliseconds > 2000)
            {
                killMe = true;
                isAlive = false;
            }
            if (isAlive)
            {
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
                speedOnX.X += changeInAcceleration.X;
                speedOnX.Y += changeInAcceleration.Y;

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

                velocity.X += Math.Pow(speedOnX.X, 5) / SPEED_DIVISOR;
                velocity.Y += Math.Pow(speedOnX.Y, 5) / SPEED_DIVISOR;

                int polarity = 0;
                polarity = velocity.X > 0 ? 1 : -1;
                position.X += velocity.X;//Math.Pow(velocity.X,1) * polarity;
                polarity = velocity.Y > 0 ? 1 : -1;
                position.Y += velocity.Y;//Math.Pow(velocity.Y,1) * polarity;
            }
        }
        public void collision(List<Player> players)
        {

            afterCollisionVelocity = new Point(velocity.X,velocity.Y);
            foreach (Player player in players)
            {
                if (player.id != this.id)//this != player might work
                {
                    if (collides(player))
                    {
                        afterCollisionVelocity.X += (player.velocity.X * Math.Abs(player.acceleration.X / 2))- (plusOrMinus(afterCollisionVelocity.X) * 1.5);
                        afterCollisionVelocity.Y += (player.velocity.Y * Math.Abs(player.acceleration.Y / 2)) - (plusOrMinus(afterCollisionVelocity.Y) * 1.5);
                    }
                }
            }
            //Set volocity according to all collisions
            //hint: dont colide with self
        }
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
        public void updateVelocity()
        {
            velocity = afterCollisionVelocity;
        }
        public bool collides(Player player)
        {
            bool doseCollide = false;
            double maxDistance = size.X / 2 + player.size.X / 2;
            if (Hypotenuse(position.X-player.position.X,position.Y-player.position.Y) <= maxDistance)
            {
                doseCollide = true;
            }
            return doseCollide;
        }
        
        public void OnPlat(MyCanvas myCanvas, Stage stage)
        {
            Point centerOfScreen = new Point(myCanvas.ActualWidth/2,myCanvas.ActualHeight/2);
            double DistanceFromCenter = Hypotenuse(centerOfScreen.X - position.X, centerOfScreen.Y - position.Y);
            
            if (DistanceFromCenter > stage.maxFromCenter+size.X/4)
            {
                isAlive = false;
                velocity = new Point(0, 0);
                acceleration = new Point(0,0);
                speedOnX = new Point(0,0);
                //Console.WriteLine("Die");
            }
        }

        
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
                    myCanvas.Children.RemoveAt(canvasItemIndex);
                    canvasItemIndex = -1;
                }
            }
        }
        static double Hypotenuse(double side1, double side2)
        {
            return Math.Sqrt(side1 * side1 + side2 * side2);
        }
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
