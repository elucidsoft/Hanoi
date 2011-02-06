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
using System.IO;
using System.Runtime.Serialization.Json;
using System.Diagnostics;
using System.Text;

namespace Hanoi
{
    public class LeaderBoardManager
    {
        WebClient wc = new WebClient();

        public event EventHandler<GetScoreCompletedEventArgs> GetScoreCompleted;

        public LeaderBoardManager()
        {
           
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(wc_DownloadStringCompleted);
            wc.UploadStringCompleted += new UploadStringCompletedEventHandler(wc_UploadStringCompleted);
        }

        void wc_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            Debug.WriteLine(e.Result);
        }

        public void GetScoreBegin()
        {
            try
            {
                wc.Headers["Content-Type"] = "";
                IsLoading = true;
                wc.DownloadStringAsync(new Uri("http://www.elucidsoft.com/__TESTING__/hanoi_highscores.php"));
            }
            catch { }
        }

        public void AddScoreBegin()
        {
            try
            {
                wc.Headers["Content-Type"] = "application/x-www-form-urlencoded";
                wc.UploadStringAsync(new Uri("http://www.elucidsoft.com/__TESTING__/hanoi_highscores.php"),
                    "POST", String.Format("action=add&did={0}&dn={1}&l={2}&m={3}&s={4}", "TEST", "5", "6", "7", "8"));
            }
            catch { }
        }

        void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(e.Result));
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(LeaderScore[]));
                var worldWideHighScores = (LeaderScore[])serializer.ReadObject(ms);

                if (GetScoreCompleted != null)
                    GetScoreCompleted(this, new GetScoreCompletedEventArgs(worldWideHighScores));

                IsLoading = false;
                Debug.WriteLine("Web service says: " + e.Result);
            }
            catch { }
        }

        public bool IsLoading { get; set; }
    }
}
