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
        public Stage()
        {
            //stageImages[0] = 
            //BitmapImage bImg = ExtBitmap.ToBitmapImage(Properties.Resources.Outterring);
            System.Drawing.Bitmap img = Properties.Resources.Outterring;
            img = new System.Drawing.Bitmap((System.Drawing.Image)img, new System.Drawing.Size((int)(img.Width * SIZE_RATIO), (int)(img.Height * SIZE_RATIO)));
            stageImages[3] = ExtBitmap.ToBitmapImage(img);
            maxSize = img.Width * SIZE_RATIO;
            maxSizes[3] = img.Width * SIZE_RATIO;
            maxFromCenters[3] = img.Width / 2;
            //maxSize = img.Width * SIZE_RATIO;
            //maxFromCenter = img.Width/2;

            img = Properties.Resources.Outring;
            img = new System.Drawing.Bitmap((System.Drawing.Image)img, new System.Drawing.Size((int)(img.Width * SIZE_RATIO), (int)(img.Height * SIZE_RATIO)));
            stageImages[2] = ExtBitmap.ToBitmapImage(img);
            maxSizes[2] = img.Width * SIZE_RATIO;
            maxFromCenters[2] = img.Width / 2;

            img = Properties.Resources.InRing;
            img = new System.Drawing.Bitmap((System.Drawing.Image)img, new System.Drawing.Size((int)(img.Width * SIZE_RATIO), (int)(img.Height * SIZE_RATIO)));
            stageImages[1] = ExtBitmap.ToBitmapImage(img);
            maxSizes[1] = img.Width * SIZE_RATIO;
            maxFromCenters[1] = img.Width / 2;

            img = Properties.Resources.inner;
            img = new System.Drawing.Bitmap((System.Drawing.Image)img, new System.Drawing.Size((int)(img.Width * SIZE_RATIO), (int)(img.Height * SIZE_RATIO)));
            stageImages[0] = ExtBitmap.ToBitmapImage(img);
            maxSizes[1] = img.Width * SIZE_RATIO;
            maxFromCenters[1] = img.Width / 2;

            //stageImages[1] = ExtBitmap.ToBitmapImage(Properties.Resources.Outring);
            //stageImages[2] = ExtBitmap.ToBitmapImage(Properties.Resources.InRing);

            //stageImages[3] = ExtBitmap.ToBitmapImage(Properties.Resources.inner);
        }
        public void DrawStage(MyCanvas canvas)
        {
            for (int i = 0; i <= state; i++)
            {
                
                Image img = new Image();
                img.Source = stageImages[i];
                img.Width = stageImages[i].Width;
                img.Height = stageImages[i].Height;
                Canvas.SetLeft(img, canvas.ActualWidth / 2 - img.Width / 2);
                Canvas.SetTop(img, canvas.ActualHeight / 2 - img.Height / 2);
                canvas.Children.Add(img);
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
                stageTime = 0;
            }
        }
    }
}
