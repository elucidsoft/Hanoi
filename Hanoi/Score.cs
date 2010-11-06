using System;
using ProtoBuf;

namespace Hanoi
{
    [ProtoContract]
    public class Score
    {
        public Score() { } //Needed for serialization

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

        [ProtoMember(1)]
        public int Level { get; set; }

        [ProtoMember(2)]
        public int Moves { get; set; }

        [ProtoMember(3)]
        public int Seconds { get; set; }

        [ProtoMember(4)]
        public DateTime Date { get; set; }

        public override string ToString()
        {             
            string display = String.Format("Level {0} - Not Played", Level);
            if (Moves > 0 && Seconds > 0)
            {
                display = String.Format("Level {0} - {1} moves in {2}",
                        Level,
                        Moves,
                        TimeSpan.FromSeconds(Seconds).ToString());
            }
            return display;
        }
    }
}
