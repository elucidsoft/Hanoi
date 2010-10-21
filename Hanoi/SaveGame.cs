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
using System.Collections.Generic;

namespace Hanoi
{
    public class SaveGame
    {
        public int Level { get; set; }
        public int Moves { get; set; }
        public int Seconds { get; set; }

        public int StackOneCount;

        public int StackTwoCount;

        public int StackThreeCount;

        public List<SaveDiscData> SaveDiscDataOne = new List<SaveDiscData>();

        public List<SaveDiscData> SaveDiscDataTwo = new List<SaveDiscData>();

        public List<SaveDiscData> SaveDiscDataThree = new List<SaveDiscData>();

    }

    public class SaveDiscData
    {
        public double Left;

        public double Top;

        public int Size;

        public DiscStack DiscStack;
        public double OriginalLeft;
        public double OriginalTop;

    }
}
