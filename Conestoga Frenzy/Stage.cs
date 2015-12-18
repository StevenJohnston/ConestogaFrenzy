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
    public class Stage
    {
        const int STAGE_LENGTH = 20 * 120;//20 seconds
        int stageTime = 0;
        const double SIZE_RATIO = 1;
        public double maxSize = 0;
        public double SPAWN_DISTANCE_RATIO = 0.75;
        public double maxFromCenter = 0;
        Point size;
        public int state =3;
        BitmapImage[] stageImages = new BitmapImage[4];
        public double[] maxSizes = new double[4];
        public double[] maxFromCenters = new double[4];
        public Image[] elements = new Image[4];
        public int[] elementIndexs = new int[4]{ -1,-1,-1,-1};
        bool[] visable = new bool[4] { true,true,true,true};
        int FlashTimer = 0;
        public Stage()
        {
            //stageImages[0] = 
            //BitmapImage bImg = ExtBitmap.ToBitmapImage(Properties.Resources.Outterring);
            System.Drawing.Bitmap img = Properties.Resources.Outterring;
            img = new System.Drawing.Bitmap((System.Drawing.Image)img, new System.Drawing.Size((int)(img.Width * SIZE_RATIO), (int)(img.Height * SIZE_RATIO)));
            stageImages[3] = ExtBitmap.ToBitmapImage(img);
            maxSize = img.Width * SIZE_RATIO;
            maxFromCenter = img.Width / 2;
            maxSizes[3] = img.Width * SIZE_RATIO;
            maxFromCenters[3] = img.Width / 2;

            elements[3] = new Image();
            elements[3].Source = stageImages[3];
            elements[3].Width = img.Width * SIZE_RATIO;
            elements[3].Height = img.Height * SIZE_RATIO;

            //maxSize = img.Width * SIZE_RATIO;
            maxFromCenter = img.Width/2;

            img = Properties.Resources.Outring;
            img = new System.Drawing.Bitmap((System.Drawing.Image)img, new System.Drawing.Size((int)(img.Width * SIZE_RATIO), (int)(img.Height * SIZE_RATIO)));
            stageImages[2] = ExtBitmap.ToBitmapImage(img);
            maxSizes[2] = img.Width * SIZE_RATIO;
            maxFromCenters[2] = img.Width / 2;
            elements[2] = new Image();
            elements[2].Source = stageImages[2];
            elements[2].Width = img.Width * SIZE_RATIO;
            elements[2].Height = img.Height * SIZE_RATIO;

            img = Properties.Resources.InRing;
            img = new System.Drawing.Bitmap((System.Drawing.Image)img, new System.Drawing.Size((int)(img.Width * SIZE_RATIO), (int)(img.Height * SIZE_RATIO)));
            stageImages[1] = ExtBitmap.ToBitmapImage(img);
            maxSizes[1] = img.Width * SIZE_RATIO;
            maxFromCenters[1] = img.Width / 2;
            elements[1] = new Image();
            elements[1].Source = stageImages[1];
            elements[1].Width = img.Width * SIZE_RATIO;
            elements[1].Height = img.Height * SIZE_RATIO;

            img = Properties.Resources.inner;
            img = new System.Drawing.Bitmap((System.Drawing.Image)img, new System.Drawing.Size((int)(img.Width * SIZE_RATIO), (int)(img.Height * SIZE_RATIO)));
            stageImages[0] = ExtBitmap.ToBitmapImage(img);
            maxSizes[0] = img.Width * SIZE_RATIO;
            maxFromCenters[0] = img.Width / 2;
            elements[0] = new Image();
            elements[0].Source = stageImages[0];
            elements[0].Width = img.Width * SIZE_RATIO;
            elements[0].Height = img.Height * SIZE_RATIO;

            //stageImages[1] = ExtBitmap.ToBitmapImage(Properties.Resources.Outring);
            //stageImages[2] = ExtBitmap.ToBitmapImage(Properties.Resources.InRing);

            //stageImages[3] = ExtBitmap.ToBitmapImage(Properties.Resources.inner);
        }

        public void Reset()
        {
            state = 3;
            maxSize = maxSizes[state];
            maxFromCenter = maxFromCenters[state];
            FlashTimer = 0;
            visable = new bool[4] { true, true, true, true };
        }

        public void DrawStage(MyCanvas canvas)
        {
            for (int i = 0; i <= state; i++)
            {

                Canvas.SetLeft(elements[i], canvas.ActualWidth / 2 - elements[i].Width / 2);
                Canvas.SetTop(elements[i], canvas.ActualHeight / 2 - elements[i].Height / 2);
                if (elementIndexs[i] == -1)
                {
                    elementIndexs[i] = canvas.Children.Add(elements[i]);
                }
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
                maxFromCenter = maxSizes[state] / 2;
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
