using System;

namespace Hanoi
{
    public class MoveCompletedEventArgs : EventArgs
    {
        public MoveCompletedEventArgs(int moves)
        {
            Moves = moves;
        }

        public int Moves { get; private set; }
    }
}
