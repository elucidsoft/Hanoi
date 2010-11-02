using System;

namespace Hanoi
{
    public class LevelTimerTickEventArgs : EventArgs
    {
        public LevelTimerTickEventArgs(int seconds)
        {
            Seconds = seconds;
        }
        public int Seconds { get; private set; }
    }
}
