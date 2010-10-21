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
using System.Text;

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
            //int sec = 0;
            //int min = 0;
            //int hr = 0;

            //if (Seconds >= 60)
            //    min = Seconds / 60;

            //if (Seconds >= 3600)
            //{
            //    hr = Seconds / 3600;
            //    min = Math.Abs(new TimeSpan(0, min, 0).Subtract(new TimeSpan(hr, 0, 0)).Minutes);
            //}

            //sec = Math.Abs(new TimeSpan(hr, min, 0).Subtract(new TimeSpan(0, 0, Seconds)).Seconds);
                
            string display = String.Format("Level {0} - Not Played", Level);
            if (Moves > 0 && Seconds > 0)
            {
            //    StringBuilder timeText = new StringBuilder();

            //    if(sec > 0)
            //    {
            //        timeText.AppendFormat("{0} sec{1}",sec, sec > 0 ? "'s" : String.Empty);
            //    }
                
            //    timeText.Append(" ");

            //    if(min > 0)
            //    {
            //        timeText.AppendFormat("{0} min{1}", min, min > 0 ? "'s" : String.Empty);
            //    }

            //    timeText.Append(" ");

            //    if (hr > 0)
            //    {
            //        timeText.AppendFormat("{0} hr{1}", hr, hr > 0 ? "'s" : String.Empty);
            //    }

                display = String.Format("Level {0} - {1} moves in {2}",
                        Level,
                        Moves,
                        TimeSpan.FromSeconds(Seconds).ToString());
            }
            return display;
        }
    }
}
