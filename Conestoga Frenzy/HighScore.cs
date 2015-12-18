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

        TextBox highScoreBox;
        List<Player> highScores = new List<Player>();
        string scoreString = "High Scores";
        public HighScore(TextBox textBox)
        {
            highScoreBox = textBox;
        }
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
