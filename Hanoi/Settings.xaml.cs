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
using System.IO.IsolatedStorage;
using Microsoft.Phone.Tasks;

namespace Hanoi
{
    public partial class Settings : PhoneApplicationPage
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void btnClearHighScores_Click(object sender, RoutedEventArgs e)
        {
            GameManager.Instance.ClearHighScores();
        }

        private void btnRateAndReview_Click(object sender, RoutedEventArgs e)
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();
            marketplaceReviewTask.Show();
        }

        private void sliderVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (txtSliderVolume != null)
            {
                double volume = Math.Round(sliderVolume.Value, 0);
                txtSliderVolume.Text = volume + "%";
                App.GameData.GameSettings.SoundVolume = volume;
            }
        }

        private void tglPlaySounds_Click(object sender, RoutedEventArgs e)
        {
            sliderVolume.IsEnabled = tglPlaySounds.IsChecked.Value;
            App.GameData.GameSettings.PlaySounds = tglPlaySounds.IsChecked.Value;
        }

        private void sliderVolume_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            txtSliderVolume.Text = (sliderVolume.IsEnabled) ? sliderVolume.Value.ToString() + "%" : "Sound is Off";
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            GameData.LoadGameData();

            tglPlaySounds.IsChecked = App.GameData.GameSettings.PlaySounds;
            sliderVolume.Value = App.GameData.GameSettings.SoundVolume;
            tglVibrateOnInvalidMove.IsChecked = App.GameData.GameSettings.VibrateOnInvalidMove;
            tglShowTitleBar.IsChecked = App.GameData.GameSettings.ShowTitleBar;
            tglShowMoveCounter.IsChecked = App.GameData.GameSettings.ShowMoveCounter;
            tglShowLevel.IsChecked = App.GameData.GameSettings.ShowLevel;
            tglShowTimer.IsChecked = App.GameData.GameSettings.ShowTimer;

            if (App.IsTrial)
            {
                lblTrialModeWarning.Visibility = Visibility.Visible;

                //Disable Play Sounds
                tglPlaySounds.IsEnabled = false;
                
                //Disable Volume Slider
                SolidColorBrush disabledBrush = (SolidColorBrush)ColorConverter.Convert(Resources["PhoneDisabledColor"]);
                //txtSliderVolume.Foreground = disabledBrush;
                //lblSoundVolume.Foreground = disabledBrush;
                sliderVolume.IsEnabled = false;

                //Disable Vibrate on Invalid Move
                tglVibrateOnInvalidMove.IsEnabled = false;

                //Disable Show Title Bar
                tglShowTitleBar.IsEnabled = false;

                //Disable Show Move Counter
                tglShowMoveCounter.IsEnabled = false;

                //Disable Show Level
                tglShowLevel.IsEnabled = false;

                //Disable Show Timer
                tglShowTimer.IsEnabled = false;

            }
        }

        private void tglShowTitleBar_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            App.GameData.GameSettings.ShowTitleBar = tglShowTitleBar.IsChecked.Value;
        }

        private void tglShowMoveCounter_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            App.GameData.GameSettings.ShowMoveCounter = tglShowMoveCounter.IsChecked.Value;
        }

        private void tglShowTimer_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            App.GameData.GameSettings.ShowTimer = tglShowTimer.IsChecked.Value;
        }

        private void tglShowLevel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            App.GameData.GameSettings.ShowLevel = tglShowLevel.IsChecked.Value;
        }

        private void tglVibrateOnInvalidMove_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            App.GameData.GameSettings.VibrateOnInvalidMove = tglVibrateOnInvalidMove.IsChecked.Value;
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            GameData.SaveGameData(App.GameData);
            App.CalculateCanContinue();
        }
    }
}