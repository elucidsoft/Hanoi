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
using System.Runtime.Serialization;

namespace Hanoi
{
    [DataContract]
    public class LeaderScore
    {
        [DataMember(Name = "did")]
        public string DeviceId_Hashed { get; set; }

        [DataMember(Name = "dn")]
        public string UserName { get; set; }

        [DataMember(Name = "l")]
        public string Level { get; set; }

        [DataMember(Name = "m")]
        public string Moves { get; set; }

        [DataMember(Name = "s")]
        public string Seconds { get; set; }

        public string Time
        {
            get
            {
                return TimeSpan.FromSeconds(Convert.ToDouble(Seconds)).ToString();
            }
        }

        public string Rank { get; set; }
    }
}
