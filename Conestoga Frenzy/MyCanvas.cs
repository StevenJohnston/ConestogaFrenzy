//File: MyCanvas.cs
//Name: Steven Johnston, Matthew Warren
//Date: 11/18/2015
//Description: 
//      inhearates from Canvas. 
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
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.Windows.Controls.Canvas" />
    public class MyCanvas : Canvas
    {

        /// <summary>
        /// The empty delegate
        /// </summary>
        private static Action EmptyDelegate = delegate () { };
        /// <summary>
        /// Called when [render].
        /// </summary>
        /// <param name="dc">The dc.</param>
        protected override void OnRender(DrawingContext dc)
        {
            
            //BitmapImage img = new BitmapImage(new Uri("c:\\demo.jpg"));
            //dc.DrawImage(img, new Rect(0, 0, img.PixelWidth, img.PixelHeight));
        }
        /// <summary>
        /// Draws the players.
        /// </summary>
        /// <param name="players">The players.</param>
        public void DrawPlayers(List<Player> players)
        {
            foreach (Player player in players)
            {
                player.Draw(this);
            }
            Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }
        /// <summary>
        /// Draws the stage.
        /// </summary>
        /// <param name="stage">The stage.</param>
        public void DrawStage(Stage stage)
        {
            
            stage.DrawStage(this);
            Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }
    }
}
