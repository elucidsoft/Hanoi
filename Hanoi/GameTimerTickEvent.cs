using System;

namespace Hanoi
{
    public class LevelTimerTickEventArgs : EventArgs
    {
        public LevelTimerTickEventArgs(long seconds)
        {
            Seconds = seconds;
        }
        public long Seconds { get; private set; }
    }
}
