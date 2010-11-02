using System;

namespace Hanoi
{
    public class HighScoreEventArgs : EventArgs
    {
        public Score Score;
        public HighScoreEventArgs(Score score)
        {
            Score = score;
        }
    }
}
