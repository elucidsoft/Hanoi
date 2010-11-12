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
using Microsoft.Xna.Framework;
using Microsoft.Phone.Tasks;

namespace Hanoi
{
    public partial class MainPage : PhoneApplicationPage
    {
        bool isNew = true;
        public MainPage()
        {
            InitializeComponent();

            btnContinue.Visibility = App.CanContinue ?
               Visibility.Visible :
               Visibility.Collapsed;

            SetupTrial();
            LoadHighScores();
            EnableLockedLevels();
        }

        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            StartLevel(1);
        }

        private void buttonLevel_Click(object sender, RoutedEventArgs e)
        {
            object obj = ((Button)sender).Content;

            if (obj is Image)
            {
                if (((Button)((Image)obj).Parent).Name == "btnLevel11")
                {
                    lblMessageBoxText.Text = "This level is locked, you must complete level 10 to play this level!";
                }
                else
                {
                    lblMessageBoxText.Text = "This level is locked, you must complete level 11 to play this level!";
                }

                lblMessageBoxTitle.Text = "Level is locked!";
                ShowMessageBox.Begin();
                return;
            }

            int level = Convert.ToInt32(obj);
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
            Dispatcher.BeginInvoke(() =>
            {
                lbHighScores.Items.Clear();
                GameManager.Instance.LoadHighScores();
                List<Score> highScores = GameManager.Instance.HighScores;
                for (int i = 0; i <= highScores.Count - 1; i++)
                {
                    Score score = highScores[i];
                    lbHighScores.Items.Add(new ListBoxItem() { Content = score.ToString() });
                }
            });
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            btnContinue.Visibility = App.CanContinue ?
                Visibility.Visible :
                Visibility.Collapsed;

            if (!isNew)
            {
                LoadHighScores();
                EnableLockedLevels();
            }
            isNew = false;
        }

        private void EnableLockedLevels()
        {
            if (GameManager.Instance.HighScores[9].Seconds > 0)
            {
                btnLevel11.Content = "11";
            }
            else
            {
                btnLevel11.Content = image_Copy1;
            }


            if (GameManager.Instance.HighScores[10].Seconds > 0)
                btnLevel12.Content = "12";
            else
                btnLevel12.Content = image_Copy;
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
                btnLevel11.IsEnabled = false;
                btnLevel12.IsEnabled = false;
                btnBuy.Visibility = Visibility.Visible;
                lblAbout.Text = "Buy the full game to experience all levels, 30+ high def background, settings, and much more!";
                //lblVersion.Text = "Hanoi v1.0 Trial Version";
            }
            else
            {
                lblAbout.Text = "Thank you for supporting our great games, have fun!";
                
                //lblVersion.Text = "Hanoi v1.0 Full Version";
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

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Settings.xaml", UriKind.Relative));
        }

        private void btnBuy_Click(object sender, RoutedEventArgs e)
        {
            MarketplaceDetailTask marketplaceDetailTask = new MarketplaceDetailTask();
            marketplaceDetailTask.Show();
        }

        private void btnMessageBoxOk_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            HideMessageBox.Begin();
        }
    }

}

