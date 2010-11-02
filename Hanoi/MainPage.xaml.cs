using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;

namespace Hanoi
{

    public partial class MainPage : PhoneApplicationPage
    {

        public MainPage()
        {
            InitializeComponent();
            SetupTrial();
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
            App.CanContinue = false;
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

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            btnContinue.Visibility = App.CanContinue ? 
                Visibility.Visible : 
                Visibility.Collapsed;

            LoadHighScores();
        }

        private void SetupTrial()
        {
            if (App.IsTrial)
            {
                btnLevel6.IsEnabled = false;
                btnLevel7.IsEnabled = false;
                btnLevel8.IsEnabled = false;
                btnLevel9.IsEnabled = false;
                btnLevel10.IsEnabled = false;
                btnSettings.IsEnabled = false;
                btnBuy.Visibility = Visibility.Visible;
                lblAbout.Text = "Buy the full game to experience all levels, 30+ high def background, settings, and much more!";
                lblVersion.Text = "Hanoi v1.0 Trial Version";
            }
            else
            {
                lblAbout.Text = "Thank you for supporting our great games, have fun!";
                lblVersion.Text = "Hanoi v1.0 Full Version";
            }
        }

        private void lbHighScores_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lbHighScores.SelectedIndex = -1;
        }

        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/GameScreen.xaml", UriKind.Relative));
        }
    }
}