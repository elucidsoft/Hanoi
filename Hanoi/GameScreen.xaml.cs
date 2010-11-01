using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
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
using System.Threading;
using System.Windows.Media.Imaging;

namespace Hanoi
{
    public partial class GameScreen : PhoneApplicationPage
    {
        AutoResetEvent messageBoxWait = new AutoResetEvent(true);
        Action messageBoxAction = () => { };

        public GameScreen()
        {
            InitializeComponent();
        }

        void Instance_LevelTimerTick(object sender, LevelTimerTickEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                tbTimer.Text = String.Format("{0:HH:mm:ss}", new DateTime(TimeSpan.FromSeconds(e.Seconds).Ticks));
            });
        }

        void Instance_MoveCompleted(object sender, MoveCompletedEventArgs e)
        {
            tbMoves.Text = e.Moves.ToString();
        }

        void Instance_LevelCompleted(object sender, System.EventArgs e)
        {
            if (messageBoxWait.WaitOne())
            {
                Dispatcher.BeginInvoke(() =>
                    {
                        tbMoves.Text = GameManager.Instance.Moves.ToString();
                        LevelTransition_Start.Begin();
                    });
            }
        }

        void LevelTransition_Start_Completed(object sender, System.EventArgs e)
        {
            ClearDiscs();

            SetBackgroundImage(false);
            GameManager.Instance.Start();
            BuildVisualStack(DiscStack.One);
            LevelTransition_End.Begin();
        }

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

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            bool isContinue = false;
            GameManager.Instance.LevelCompleted += new System.EventHandler(Instance_LevelCompleted);
            GameManager.Instance.MoveCompleted += new System.EventHandler<MoveCompletedEventArgs>(Instance_MoveCompleted);
            GameManager.Instance.LevelTimerTick += new System.EventHandler<LevelTimerTickEventArgs>(Instance_LevelTimerTick);
            GameManager.Instance.HighScore += new System.EventHandler<HighScoreEventArgs>(Instance_HighScore);
            GameManager.Instance.TrialModeCompleted += new System.EventHandler<System.EventArgs>(Instance_TrialModeCompleted);
            LevelTransition_Start.Completed += new System.EventHandler(LevelTransition_Start_Completed);
            ShowMessageBox.Completed += new System.EventHandler(ShowMessageBox_Completed);
            HideMessageBox.Completed += new System.EventHandler(HideMessageBox_Completed);


            if (App.CanContinue)
            {
                GameManager.Instance.LoadStateData();
                App.CanContinue = false;
                isContinue = true;
            }

            GameManager.Instance.Start();
            BuildVisualStack(DiscStack.One);
            BuildVisualStack(DiscStack.Two);
            BuildVisualStack(DiscStack.Three);
            tbMoves.Text = GameManager.Instance.Moves.ToString();
            SetBackgroundImage(isContinue);
        }

        void Instance_TrialModeCompleted(object sender, System.EventArgs e)
        {
            messageBoxWait.Reset();
            lblMessageBoxTitle.Text = "Trial Game Complete!";
            lblMessageBoxText.Text = "Upgrade to the full game, see marketplace for more details.";
            ShowMessageBox.Begin();
            messageBoxAction = () =>
            {
                GameManager.Instance.Reset();
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            };
        }

        void HideMessageBox_Completed(object sender, System.EventArgs e)
        {
            messageBoxAction();
            messageBoxWait.Set();
        }

        void ShowMessageBox_Completed(object sender, System.EventArgs e)
        {
            messageBoxWait.Reset();
        }

        void Instance_HighScore(object sender, HighScoreEventArgs he)
        {
            lblMessageBoxTitle.Text = "You achieved a new high score!";
            lblMessageBoxText.Text = String.Format("{0} Moves in {1}", he.Score.Moves, TimeSpan.FromSeconds(he.Score.Seconds).ToString());
            ShowMessageBox.Begin();
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

        private void phoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            App.CanContinue = true;
            GameManager.Instance.SaveState();
        }

        private void phoneApplicationPage_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            GameManager.Instance.LevelCompleted -= new System.EventHandler(Instance_LevelCompleted);
            GameManager.Instance.MoveCompleted -= new System.EventHandler<MoveCompletedEventArgs>(Instance_MoveCompleted);
            GameManager.Instance.LevelTimerTick -= new System.EventHandler<LevelTimerTickEventArgs>(Instance_LevelTimerTick);
            LevelTransition_Start.Completed -= new System.EventHandler(LevelTransition_Start_Completed);
            GameManager.Instance.HighScore -= new System.EventHandler<HighScoreEventArgs>(Instance_HighScore);
            ShowMessageBox.Completed -= new System.EventHandler(ShowMessageBox_Completed);
            HideMessageBox.Completed -= new System.EventHandler(HideMessageBox_Completed);
        }

        private void btnMessageBoxOk_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            HideMessageBox.Begin();
        }
    }
}