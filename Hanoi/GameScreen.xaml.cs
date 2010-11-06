using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Controls;

namespace Hanoi
{
    public partial class GameScreen : PhoneApplicationPage, IDisposable
    {
        bool disposed = false;
        ManualResetEvent messageBoxWait = new ManualResetEvent(true);
        Action messageBoxAction = () => { };

        #region ctor/Load & Unload

        public GameScreen()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            bool isContinue = false;
            GameManager.Instance.LevelCompleted += new System.EventHandler(Instance_LevelCompleted);
            GameManager.Instance.MoveCompleted += new System.EventHandler<MoveCompletedEventArgs>(Instance_MoveCompleted);
            GameManager.Instance.LevelTimerTick += new System.EventHandler<LevelTimerTickEventArgs>(Instance_LevelTimerTick);
            GameManager.Instance.HighScore += new System.EventHandler<HighScoreEventArgs>(Instance_HighScore);
            GameManager.Instance.TrialModeCompleted += new System.EventHandler<System.EventArgs>(Instance_TrialModeCompleted);
            LevelTransition_Start.Completed += new System.EventHandler(LevelTransition_Start_Completed);
            LevelTransition_End.Completed += new EventHandler(LevelTransition_End_Completed);
            ShowMessageBox.Completed += new System.EventHandler(ShowMessageBox_Completed);
            HideMessageBox.Completed += new System.EventHandler(HideMessageBox_Completed);


            if (App.CanContinue)
            {
                GameManager.Instance.LoadStateData();
                App.CanContinue = false;
                isContinue = true;
            }

            GameManager.Instance.Start(false);
            BuildVisualStack(DiscStack.One);
            BuildVisualStack(DiscStack.Two);
            BuildVisualStack(DiscStack.Three);
            tbMoves.Text = GameManager.Instance.Moves.ToString();
            SetBackgroundImage(isContinue);

            if (App.GameData.GameSettings.ShowTitleBar == false)
            {
                gridTitleBar.Visibility = Visibility.Collapsed;
            }
            else
            {
                gridLevel.Visibility = App.GameData.GameSettings.ShowLevel ? Visibility.Visible : Visibility.Collapsed;
                gridMoves.Visibility = App.GameData.GameSettings.ShowMoveCounter ? Visibility.Visible : Visibility.Collapsed;
                gridTimer.Visibility = App.GameData.GameSettings.ShowTimer ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void phoneApplicationPage_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            GameManager.Instance.LevelCompleted -= new System.EventHandler(Instance_LevelCompleted);
            GameManager.Instance.MoveCompleted -= new System.EventHandler<MoveCompletedEventArgs>(Instance_MoveCompleted);
            GameManager.Instance.LevelTimerTick -= new System.EventHandler<LevelTimerTickEventArgs>(Instance_LevelTimerTick);
            GameManager.Instance.HighScore -= new System.EventHandler<HighScoreEventArgs>(Instance_HighScore);
            GameManager.Instance.TrialModeCompleted -= new System.EventHandler<System.EventArgs>(Instance_TrialModeCompleted);
            LevelTransition_Start.Completed -= new System.EventHandler(LevelTransition_Start_Completed);
            LevelTransition_End.Completed -= new EventHandler(LevelTransition_End_Completed);
            ShowMessageBox.Completed -= new System.EventHandler(ShowMessageBox_Completed);
            HideMessageBox.Completed -= new System.EventHandler(HideMessageBox_Completed);
        }

        #endregion

        #region Events

        void Instance_LevelTimerTick(object sender, LevelTimerTickEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                tbTimer.Text = String.Format("{0:HH:mm:ss}", new DateTime(TimeSpan.FromSeconds(e.Seconds).Ticks));
            });
        }

        void Instance_MoveCompleted(object sender, MoveCompletedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                tbMoves.Text = e.Moves.ToString();
            });
        }

        void Instance_LevelCompleted(object sender, System.EventArgs e)
        {
            if (messageBoxWait.WaitOne())
            {
                Dispatcher.BeginInvoke(() =>
                    {
                        blockInteractionCanvas.Visibility = Visibility.Visible;
                        tbMoves.Text = GameManager.Instance.Moves.ToString();
                        LevelTransition_Start.Begin();
                    });
            }
        }

        void LevelTransition_Start_Completed(object sender, System.EventArgs e)
        {
            ClearDiscs();

            SetBackgroundImage(false);
            GameManager.Instance.Start(true);
            BuildVisualStack(DiscStack.One);

            Dispatcher.BeginInvoke(() =>
            {
                LevelTransition_End.Begin();
            });
        }

        void LevelTransition_End_Completed(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
           {
               blockInteractionCanvas.Visibility = Visibility.Collapsed;
               GameManager.Instance.StartTimer();
           });
        }

        void Instance_TrialModeCompleted(object sender, System.EventArgs e)
        {
            messageBoxWait.Reset();
            lblMessageBoxTitle.Text = "Trial Game Complete!";
            lblMessageBoxText.Text = "For more details on upgrading to the full version visit the marketplace.";
            ShowMessageBox.Begin();
            messageBoxAction = () =>
            {
                App.CanContinue = false;
                NavigationService.GoBack();
            };
        }



        void Instance_HighScore(object sender, HighScoreEventArgs he)
        {
            lblMessageBoxTitle.Text = "You achieved a new high score!";
            lblMessageBoxText.Text = String.Format("{0} Moves in {1}", he.Score.Moves, TimeSpan.FromSeconds(he.Score.Seconds).ToString());
            ShowMessageBox.Begin();
        }

        private void phoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            App.CanContinue = true;
            GameManager.Instance.SaveState();
        }

        #endregion

        #region UI Methods

        void SetBackgroundImage(bool isContinue)
        {
                if (!isContinue)
                {
                    const int max = 29;
                    Image img = (bgImage01.Opacity == 1.0) ? bgImage01 : bgImage02;
                    Image img2 = (bgImage01.Opacity == 1.0) ? bgImage02 : bgImage01;
                    Random rnd = new Random();
                    string file1 = String.Format("{0:0#}.jpg", rnd.Next(1, max));
                    img.Source = new BitmapImage(new Uri(@"\images\backgrounds\normal\" + file1, System.UriKind.Relative));
                    img2.Source = new BitmapImage(new Uri(@"\images\backgrounds\normal\" + String.Format("{0:0#}.jpg", rnd.Next(1, max)), System.UriKind.Relative));
                    App.GameData.SaveGame.BackgroundImage = file1;
                }
                else
                {
                    bgImage01.Source = new BitmapImage(new Uri(@"\images\backgrounds\normal\" + App.GameData.SaveGame.BackgroundImage, System.UriKind.Relative));
                }
        }

        private void ClearDiscs()
        {
            for (int i = canvas.Children.Count - 1; i != 0; i--)
            {
                UIElement h = canvas.Children[i];
                if (h is HanoiDisc)
                    canvas.Children.Remove(h);
            }
        }

        private void BuildVisualStack(DiscStack stack)
        {
            tbLevel.Text = "Level " + GameManager.Instance.Level;
            IList<HanoiDisc> discs = GameManager.Instance.GetDiscsInStack(stack);
            for (int i = 0; i <= discs.Count - 1; i++)
            {
                canvas.Children.Add(discs[i]);
            }
        }

        #endregion

        #region MessageBox

        private void btnMessageBoxOk_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            HideMessageBox.Begin();
            blockInteractionCanvas.Visibility = Visibility.Collapsed;
        }

        void HideMessageBox_Completed(object sender, System.EventArgs e)
        {
            messageBoxAction();
            messageBoxWait.Set();
        }

        void ShowMessageBox_Completed(object sender, System.EventArgs e)
        {
            blockInteractionCanvas.Visibility = Visibility.Visible;
            messageBoxWait.Reset();
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    messageBoxWait.Dispose();
                }

                disposed = true;
            }
        }

        ~GameScreen()
        {
            Dispose(false);
        }

        #endregion
    }
}