//File: Stage.cs
//Name: Steven Johnston, Matthew Warren
//Date: 11/18/2015
//Description: 
//      The stage the players play on
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Conestoga_Frenzy
{
    /// <summary>
    /// 
    /// </summary>
    public class Stage
    {
        /// <summary>
        /// Number of updates ticks before stage changes
        /// </summary>
        const int STAGE_LENGTH = 20 * 120;//20 seconds
        /// <summary>
        /// Curent time on stage
        /// </summary>
        int stageTime = 0;
        /// <summary>
        /// The state size ratio
        /// </summary>
        const double SIZE_RATIO = 0.9;
        /// <summary>
        /// The maximum size of stage
        /// </summary>
        public double maxSize = 0;
        /// <summary>
        /// How from from the middle to the edge plays should spawn
        /// </summary>
        public double SPAWN_DISTANCE_RATIO = 0.75;//0.75
        /// <summary>
        /// The maximum distance from center before player falls off
        /// </summary>
        public double maxFromCenter = 0;
        /// <summary>
        /// The state of the game 3= all rings 0= 1 ring(center)
        /// </summary>
        public int state =3;
        /// <summary>
        /// The stage images (rings)
        /// </summary>
        BitmapImage[] stageImages = new BitmapImage[4];
        /// <summary>
        /// The size of each ring
        /// </summary>
        public double[] maxSizes = new double[4];
        /// <summary>
        /// The maximum distance from center of each stage
        /// </summary>
        public double[] maxFromCenters = new double[4];
        /// <summary>
        /// The elements(rings) to be drawn to canvase
        /// </summary>
        public Image[] elements = new Image[4];
        /// <summary>
        /// The element indexs
        /// </summary>
        public int[] elementIndexs = new int[4]{ -1,-1,-1,-1};
        /// <summary>
        /// The visablity of each ring
        /// </summary>
        bool[] visable = new bool[4] { true,true,true,true};
        /// <summary>
        /// The flash timer
        /// </summary>
        int FlashTimer = 0;
        /// <summary>
        /// Initializes a new instance of the <see cref="Stage"/> class.
        /// </summary>
        public Stage()
        {
            //Gets image for outter ring
            System.Drawing.Bitmap img = Properties.Resources.Outterring;
            img = new System.Drawing.Bitmap((System.Drawing.Image)img, new System.Drawing.Size((int)(img.Width * SIZE_RATIO), (int)(img.Height * SIZE_RATIO)));
            stageImages[3] = ExtBitmap.ToBitmapImage(img);
            maxSize = img.Width * SIZE_RATIO;
            maxFromCenter = img.Width * SIZE_RATIO / 2;
            maxSizes[3] = img.Width * SIZE_RATIO;
            maxFromCenters[3] = img.Width * SIZE_RATIO / 2;

            elements[3] = new Image();
            elements[3].Source = stageImages[3];
            elements[3].Width = img.Width * SIZE_RATIO;
            elements[3].Height = img.Height * SIZE_RATIO;

            
            //Gets image for 2nd outter ring
            img = Properties.Resources.Outring;
            img = new System.Drawing.Bitmap((System.Drawing.Image)img, new System.Drawing.Size((int)(img.Width * SIZE_RATIO), (int)(img.Height * SIZE_RATIO)));
            stageImages[2] = ExtBitmap.ToBitmapImage(img);
            maxSizes[2] = img.Width * SIZE_RATIO;
            maxFromCenters[2] = img.Width * SIZE_RATIO / 2;
            elements[2] = new Image();
            elements[2].Source = stageImages[2];
            elements[2].Width = img.Width * SIZE_RATIO;
            elements[2].Height = img.Height * SIZE_RATIO;

            //gets imge from 2nd inRing
            img = Properties.Resources.InRing;
            img = new System.Drawing.Bitmap((System.Drawing.Image)img, new System.Drawing.Size((int)(img.Width * SIZE_RATIO), (int)(img.Height * SIZE_RATIO)));
            stageImages[1] = ExtBitmap.ToBitmapImage(img);
            maxSizes[1] = img.Width * SIZE_RATIO;
            maxFromCenters[1] = img.Width * SIZE_RATIO / 2;
            elements[1] = new Image();
            elements[1].Source = stageImages[1];
            elements[1].Width = img.Width * SIZE_RATIO;
            elements[1].Height = img.Height * SIZE_RATIO;

            //Gets img for inner
            img = Properties.Resources.inner;
            img = new System.Drawing.Bitmap((System.Drawing.Image)img, new System.Drawing.Size((int)(img.Width * SIZE_RATIO), (int)(img.Height * SIZE_RATIO)));
            stageImages[0] = ExtBitmap.ToBitmapImage(img);
            maxSizes[0] = img.Width * SIZE_RATIO;
            maxFromCenters[0] = img.Width * SIZE_RATIO / 2;
            elements[0] = new Image();
            elements[0].Source = stageImages[0];
            elements[0].Width = img.Width * SIZE_RATIO;
            elements[0].Height = img.Height * SIZE_RATIO;

            //stageImages[1] = ExtBitmap.ToBitmapImage(Properties.Resources.Outring);
            //stageImages[2] = ExtBitmap.ToBitmapImage(Properties.Resources.InRing);

            //stageImages[3] = ExtBitmap.ToBitmapImage(Properties.Resources.inner);
        }

        /// <summary>
        /// Resets stage;
        /// </summary>
        public void Reset()
        {
            state = 3;
            maxSize = maxSizes[state];
            maxFromCenter = maxFromCenters[state];
            FlashTimer = 0;
            stageTime = 0;
            visable = new bool[4] { true, true, true, true };
        }

        /// <summary>
        /// Draws the stage.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        public void DrawStage(MyCanvas canvas)
        {
            for (int i = 0; i <= state; i++)
            {
                //Center image
                Canvas.SetLeft(elements[i], canvas.ActualWidth / 2 - elements[i].Width / 2);
                Canvas.SetTop(elements[i], canvas.ActualHeight / 2 - elements[i].Height / 2);
                if (elementIndexs[i] == -1)
                {
                    elementIndexs[i] = canvas.Children.Add(elements[i]);
                }
                //Flicker the edge before removal
                if (visable[i])
                {
                    elements[i].Visibility = Visibility.Visible;
                }
                else
                {
                    elements[i].Visibility = Visibility.Hidden;
                }
            }
            for (int i = 3; i > state; i--)
            {
                elements[i].Visibility = Visibility.Hidden;
            }
        }
        /// <summary>
        /// Updates state(removes rings over time)
        /// </summary>
        public void Update()
        {
            stageTime++;
            if (stageTime > STAGE_LENGTH)
            {
                if (state > 0)
                {
                    state--;
                }
                maxSize = maxSizes[state];
                maxFromCenter = maxFromCenters[state];
                stageTime = 0;
            }
            else if (stageTime > STAGE_LENGTH - (120 * 5))
            {
                if (state > 0)
                {
                    FlashTimer++;
                    if (FlashTimer > 30)
                    {
                        FlashTimer = 0;
                        visable[state] = !visable[state];
                    }
                }
            }
        }
    }
}
