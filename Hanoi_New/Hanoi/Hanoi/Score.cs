using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace Hanoi
{
    public class Score
    {
        public Score()
        {

        }

        public Score(int level)
        {
            Level = level;
        }

        public Score(int level, int moves, int seconds, DateTime date)
        {
            Level = level;
            Moves = moves;
            Seconds = seconds;
            Date = date;
        }

        public int Level { get; set; }
        public int Moves { get; set; }
        public int Seconds { get; set; }
        public DateTime Date { get; set; }

        public override string ToString()
        {
            string display = String.Format("Level {0} - Not Played", Level);
            if (Moves > 0 && Seconds > 0)
            {
                display = String.Format("Level {0} - {3}: \n  Moves: {1} Time: {2:HH:mm:ss}",
                        Level,
                        Moves,
                        new DateTime(TimeSpan.FromSeconds(Seconds).Ticks),
                        Date.ToString("d"));
            }
            return display;
        }
    }
}
