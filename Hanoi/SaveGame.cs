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
using ProtoBuf;

namespace Hanoi
{
    [ProtoContract]
    public class SaveGame
    {
        [ProtoMember(1)]
        public int Level { get; set; }

        [ProtoMember(2)]
        public int Moves { get; set; }

        [ProtoMember(3)]
        public int Seconds { get; set; }

        [ProtoMember(4)]
        public int StackOneCount;

        [ProtoMember(5)]
        public int StackTwoCount;

        [ProtoMember(6)]
        public int StackThreeCount;

        [ProtoMember(7)]
        public List<SaveDiscData> SaveDiscDataOne = new List<SaveDiscData>();

        [ProtoMember(8)]
        public List<SaveDiscData> SaveDiscDataTwo = new List<SaveDiscData>();

        [ProtoMember(9)]
        public List<SaveDiscData> SaveDiscDataThree = new List<SaveDiscData>();

        [ProtoMember(10)]
        public string BackgroundImage { get; set; }
    }

    [ProtoContract]
    public class SaveDiscData
    {
        [ProtoMember(1)]
        public double Left;

        [ProtoMember(2)]
        public double Top;

        [ProtoMember(3)]
        public int Size;

        [ProtoMember(4)]
        public DiscStack DiscStack;

        [ProtoMember(5)]
        public double OriginalLeft;

        [ProtoMember(6)]
        public double OriginalTop;

    }
}
