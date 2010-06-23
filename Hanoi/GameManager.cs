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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Phone;
using Microsoft.Devices;
using Microsoft.Phone.Controls;
using System.Threading;

namespace Hanoi
{
    public enum DiscStack { One, Two, Three };

    public class GameManager
    {
        private static GameManager instance;
        public event EventHandler LevelCompleted;

        Dictionary<DiscStack, Stack<HanoiDisc>> stacks = new Dictionary<DiscStack, Stack<HanoiDisc>>();
        Dictionary<int, double> stackRows = new Dictionary<int, double>();
        Dictionary<DiscStack, double> stackColumns = new Dictionary<DiscStack, double>();
        PhoneApplicationFrame phoneAppFrame = (Application.Current.RootVisual as PhoneApplicationFrame);

        private const double virtualColumnWidth = 266;
        private const double virtualContainerCount = 3;
        private const double topStart = 350;
        private const double topSpacing = 42;
        private const double leftSpacing = 28;
        public int winCount = 0;

        private GameManager()
        {
            Initialize();
        }

        private void Initialize()
        {
            stacks.Add(DiscStack.One, new Stack<HanoiDisc>());
            stacks.Add(DiscStack.Two, new Stack<HanoiDisc>());
            stacks.Add(DiscStack.Three, new Stack<HanoiDisc>());

            stackColumns.Add(DiscStack.One, leftSpacing);
            stackColumns.Add(DiscStack.Two, virtualColumnWidth + leftSpacing - 4);
            stackColumns.Add(DiscStack.Three, (virtualColumnWidth * 2) + leftSpacing - 4);
        }

        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new GameManager();
                return instance;
            }
        }

        public void ResetForNextLevel()
        {
            stacks.Clear();
            stackRows.Clear();
            stackColumns.Clear();
            winCount = 0;
            Initialize();
        }

        public void BuildFirstStack(int level)
        {
            Stack<HanoiDisc> column1 = stacks[DiscStack.One];
            column1.Clear();


            for (int i = 0; i <= (level + 1); i++)
            {
                double left = leftSpacing;
                double top = topStart;

                HanoiDisc hanoiDisc = new HanoiDisc();
                hanoiDisc.Color = i % 2 > 0 ? "Gold" : "Grey";

                hanoiDisc.SetValue(Grid.ColumnProperty, 0);
                hanoiDisc.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                hanoiDisc.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

                if (i > 0)
                {
                    double scalex = (i * .07);
                    double scaley = (i * .05);

                    hanoiDisc.RenderTransform = new ScaleTransform() { ScaleX = 1 - scalex, ScaleY = 1 - scaley };
                    HanoiDisc previousDisc = column1.Peek();
                    top = (double)column1.Peek().GetValue(Canvas.TopProperty) - (topSpacing - (topSpacing * scaley));
                    left = leftSpacing + ((double)column1.Peek().LayoutRoot.Width * scalex) / 2;

                    hanoiDisc.ScaleX = scalex;
                    hanoiDisc.ScaleY = scaley;
                }

                hanoiDisc.SetValue(Canvas.LeftProperty, left);
                hanoiDisc.SetValue(Canvas.TopProperty, top);
                hanoiDisc.SetValue(Canvas.ZIndexProperty, 100 + i);
                hanoiDisc.DiscStack = DiscStack.One;
                hanoiDisc.Size = i;
                stackRows.Add(i, top);
                column1.Push(hanoiDisc);
                winCount++;
            }
        }

        public void MoveDiscToStack(HanoiDisc disc, DiscStack toStack)
        {
            if (!IsValidMove(disc, toStack))
                return;

            stacks[disc.DiscStack].Pop();
            double top = topStart;

            if (stacks[toStack].Count > 0)
            {
                top = (double)stacks[toStack].Peek().GetValue(Canvas.TopProperty) - (topSpacing - (topSpacing * disc.ScaleY));
            }

            double left = stackColumns[toStack] + ((double)disc.LayoutRoot.Width * disc.ScaleX) / 2;
            disc.SetValue(Canvas.TopProperty, top);
            disc.SetValue(Canvas.LeftProperty, left);

            stacks[toStack].Push(disc);
            disc.DiscStack = toStack;

            disc.ResetDirty();

            CheckForWin();
        }

        private void CheckForWin()
        {
            if (stacks[DiscStack.Three].Count == winCount)
            {
                TimerCallback tcb = BeginReset;
                Timer timer = new Timer(tcb, null, 1000, Timeout.Infinite);
            }
        }

        private void BeginReset(object obj)
        {
            ResetForNextLevel();
            if (LevelCompleted != null)
            {
                LevelCompleted(this, EventArgs.Empty);
            }
        }

        private bool IsValidMove(HanoiDisc disc, DiscStack toStack)
        {
            if (stacks[toStack].Count > 0 && stacks[toStack].Peek().Size > disc.Size)
            {
                disc.SetValue(Canvas.TopProperty, disc.OriginalTop);
                disc.SetValue(Canvas.LeftProperty, disc.OriginalLeft);
                VibrateController.Default.Start(TimeSpan.FromSeconds(1));
                return false;
            }

            return true;
        }

        public HanoiDisc GetDiscBelowCurrent(HanoiDisc current)
        {
            HanoiDisc[] discs = stacks[current.DiscStack].ToArray();
            int index = Array.IndexOf(discs, current);
            if (index - 1 > 0)
            {
                return discs[index - 1];
            }

            return null;
        }

        public int GetDiscIndex(HanoiDisc current)
        {
            HanoiDisc[] discs = stacks[current.DiscStack].ToArray();
            return Array.IndexOf(discs, current);
        }

        public bool IsCurrentDiscOnTop(HanoiDisc current)
        {
            if (stacks[current.DiscStack].Peek() == current)
                return true;

            return false;
        }

        public void SetCurrentDiscStack(HanoiDisc current)
        {
            DiscStack stack = current.DiscStack;
            double currentWidth = current.LayoutRoot.Width;
            double left = (double)current.GetValue(Canvas.LeftProperty) + ((currentWidth - (currentWidth * current.ScaleX)) / 2);
            if (left >= 0 && left <= 266)
            {
                stack = DiscStack.One;
            }
            else if (left >= 267 && left <= 532)
            {
                stack = DiscStack.Two;
            }
            else if (left >= 533)
            {
                stack = DiscStack.Three;
            }
            MoveDiscToStack(current, stack);
        }

        public ReadOnlyCollection<HanoiDisc> GetDiscsInStack(DiscStack stack)
        {
            return new ReadOnlyCollection<HanoiDisc>(stacks[stack].ToArray());
        }
    }
}
