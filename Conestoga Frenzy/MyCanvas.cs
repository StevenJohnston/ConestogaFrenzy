using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Conestoga_Frenzy
{
    public class MyCanvas : Canvas
    {
        List<Player> livingPlayers = new List<Player>();
        
        private static Action EmptyDelegate = delegate () { };
        protected override void OnRender(DrawingContext dc)
        {
            
            //BitmapImage img = new BitmapImage(new Uri("c:\\demo.jpg"));
            //dc.DrawImage(img, new Rect(0, 0, img.PixelWidth, img.PixelHeight));
        }
        public void DrawPlayers(List<Player> players)
        {
            foreach (Player player in players)
            {
                player.Draw(this);
            }
            Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }
        public void DrawStage(Stage stage)
        {
            
            stage.DrawStage(this);
            Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }
    }
}
