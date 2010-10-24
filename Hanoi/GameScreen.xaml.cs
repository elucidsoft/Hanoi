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

namespace Hanoi
{
    public partial class GameScreen : PhoneApplicationPage
    {
        public GameScreen()
        {
            InitializeComponent();
            GameManager.Instance.LevelCompleted += new System.EventHandler(Instance_LevelCompleted);
            GameManager.Instance.MoveCompleted += new System.EventHandler<MoveCompletedEventArgs>(Instance_MoveCompleted);
            GameManager.Instance.LevelTimerTick += new System.EventHandler<LevelTimerTickEventArgs>(Instance_LevelTimerTick);
            //Application.Current.Host.Settings.EnableFrameRateCounter = true;
            //Application.Current.Host.Settings.EnableRedrawRegions = true;
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
            Dispatcher.BeginInvoke(() =>
                {


                    tbMoves.Text = GameManager.Instance.Moves.ToString();

                    for (int i = canvas.Children.Count - 1; i != 0; i--)
                    {
                        UIElement h = canvas.Children[i];
                        if (h is HanoiDisc)
                            canvas.Children.Remove(h);
                    }

                    ClearDiscs();
                    GameManager.Instance.Start();
                    BuildVisualStack(DiscStack.One);
                });
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

            GameManager.Instance.Start();
            BuildVisualStack(DiscStack.One);
            BuildVisualStack(DiscStack.Two);
            BuildVisualStack(DiscStack.Three);
            tbMoves.Text = GameManager.Instance.Moves.ToString();
        }

        private void BuildVisualStack(DiscStack stack)
        {
            tbLevel.Text = "Level " + GameManager.Instance.Level;
            IList<HanoiDisc> discs = GameManager.Instance.GetDiscsInStack(stack);
            for (int i = 0; i <= discs.Count - 1; i++)
            {
                //if (!canvas.Children.Contains(discs[i]))
                //{
                //    canvas.Children.Remove();
                //}
                canvas.Children.Add(discs[i]);
            }
        }
    }
}