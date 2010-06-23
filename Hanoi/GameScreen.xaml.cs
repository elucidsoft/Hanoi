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

namespace Hanoi
{
    public partial class GameScreen : PhoneApplicationPage
    {
        private const int level = 2;
        public GameScreen()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            GameManager.Instance.BuildFirstStack(level);
            BuildVisualStack(DiscStack.One);
        }

        private void BuildVisualStack(DiscStack stack)
        {
            IList<HanoiDisc> discs = GameManager.Instance.GetDiscsInStack(stack);
            for (int i = 0; i <= discs.Count - 1; i++)
            {
                canvas.Children.Add(discs[i]);
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            for (int i = canvas.Children.Count - 1; i != 0; i--)
            {
                UIElement h = canvas.Children[i];
                if (h is HanoiDisc)
                    canvas.Children.Remove(h);
            }
            GameManager.Instance.BuildFirstStack(level);
            BuildVisualStack(DiscStack.One);
        }
    }
}