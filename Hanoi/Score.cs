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

        public Score(int level, int moves, int seconds)
        {
            Level = level;
            Moves = moves;
            Seconds = seconds;
        }

        public int Level { get; set; }
        public int Moves { get; set; }
        public int Seconds { get; set; }
    }
}
