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
        private int seconds = 0;
        
        Timer timer;

        public GameScreen()
        {
            InitializeComponent();
            GameManager.Instance.LevelCompleted += new System.EventHandler(Instance_LevelCompleted);
            GameManager.Instance.MoveCompleted += new System.EventHandler(Instance_MoveCompleted);
        }

        void Instance_MoveCompleted(object sender, System.EventArgs e)
        {
            tbMoves.Text = GameManager.Instance.Moves.ToString();
        }

        void Instance_LevelCompleted(object sender, System.EventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
                {
                    timer.Change(Timeout.Infinite, Timeout.Infinite);
                    timer.Dispose();
                    seconds = 0;
                    
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

        private void Timer_Tick(object obj)
        {
            Dispatcher.BeginInvoke(() =>
            {
                tbTimer.Text = String.Format("{0:HH:mm:ss}", new DateTime(TimeSpan.FromSeconds(seconds).Ticks));
                seconds++;
            });
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            GameManager.Instance.Start();
            BuildVisualStack(DiscStack.One);
        }

        private void BuildVisualStack(DiscStack stack)
        {
            TimerCallback tcb = Timer_Tick;
            timer = new Timer(tcb, null, 0, 1000);
            tbLevel.Text = "Level " + GameManager.Instance.Level;
            IList<HanoiDisc> discs = GameManager.Instance.GetDiscsInStack(stack);
            for (int i = 0; i <= discs.Count - 1; i++)
            {
                canvas.Children.Add(discs[i]);
            }
        }
    }
}