using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace Hanoi
{
    public partial class MainPage : PhoneApplicationPage
    {

        public MainPage()
        {
            InitializeComponent();

            SupportedOrientations = SupportedPageOrientation.Landscape;
        }

        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            StartLevel(1);
        }

        private void buttonLevel_Click(object sender, RoutedEventArgs e)
        {
            int level = Convert.ToInt32(((Button)sender).Content);
            StartLevel(level);
        }


        private void StartLevel(int level)
        {
            GameManager.Instance.Level = level;
            NavigationService.Navigate(new Uri("/GameScreen.xaml", UriKind.Relative));
        }

        private void LoadHighScores()
        {
            lbHighScores.Items.Clear();
            List<Score> highScores = GameManager.Instance.HighScores;
            for (int i = 0; i <= highScores.Count - 1; i++)
            {
                Score score = highScores[i];
                lbHighScores.Items.Add(score.ToString());
            }
        }

        private void lbHighScores_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadHighScores();
        }

        private void lbHighScores_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lbHighScores.SelectedIndex = -1;
        }
    }
}