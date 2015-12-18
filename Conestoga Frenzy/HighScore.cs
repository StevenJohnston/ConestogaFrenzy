//File: HighScore.cs
//Name: Steven Johnston, Matthew Warren
//Date: 11/18/2015
//Description: 
//      High score class to update hichscore on canvas

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Conestoga_Frenzy
{
    class HighScore
    {

        /// <summary>
        /// The high score text box
        /// </summary>
        TextBox highScoreBox;
        /// <summary>
        /// The high scores list
        /// </summary>
        List<Player> highScores = new List<Player>();
        /// <summary>
        /// The score string 
        /// </summary>
        string scoreString = "High Scores";
        /// <summary>
        /// Initializes a new instance of the <see cref="HighScore"/> class.
        /// </summary>
        /// <param name="textBox">The text box.</param>
        public HighScore(TextBox textBox)
        {
            highScoreBox = textBox;
        }
        /// <summary>
        /// Updates the player in the highscore list.
        /// </summary>
        /// <param name="player">The player.</param>
        public void updatePlayer(Player player)
        {
            scoreString = "High Scores \r\nColour      Score\r\n";
            int index = highScores.Count(x=>x.id == player.id);
            if (index > 0)
            {
                highScores.Add(player);
            }
            highScores.RemoveAll(x=>x.id == player.id);
            highScores.Add(player);

            highScores = highScores.OrderBy(x => x.score).ToList();
            
            highScores.Reverse();
            highScores.ForEach(x => scoreString += x.colour + " " + x.score + "\r\n");
            
            highScoreBox.Text = scoreString;
        }
    }
}
