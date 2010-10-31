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
using System.IO.IsolatedStorage;
using System.Xml.Serialization;
using System.Diagnostics;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework;
using System.IO;
using Microsoft.Xna.Framework.Audio;

namespace Hanoi
{
    public enum DiscStack { One, Two, Three };

    public class GameManager
    {
        private static GameManager instance;
        public event EventHandler LevelCompleted;
        public event EventHandler<MoveCompletedEventArgs> MoveCompleted;
        public event EventHandler<LevelTimerTickEventArgs> LevelTimerTick;

        Dictionary<DiscStack, Stack<HanoiDisc>> stacks = new Dictionary<DiscStack, Stack<HanoiDisc>>();
        Dictionary<double, double> stackRows = new Dictionary<double, double>();
        Dictionary<DiscStack, double> stackColumns = new Dictionary<DiscStack, double>();
        PhoneApplicationFrame phoneAppFrame = (Application.Current.RootVisual as PhoneApplicationFrame);
        List<Score> highScores = new List<Score>();

        private const string highScoreFileName = "highscores.xml";

        private const double virtualColumnWidth = 266;
        private const double virtualContainerCount = 3;
        private const double topStart = 350;
        private const double topSpacing = 42;
        private const double leftSpacing = 28;
        private int level = 1;
        private int moves = 0;
        private int seconds = 0;
        private Timer timer;
        private SoundEffect effect;

        public int winCount = 0;

        private bool isReLoaded = false;

        private GameManager()
        {
            Initialize();
        }

        ~GameManager()
        {
            effect.Dispose();
        }

        private void Initialize()
        {
            using (Stream stream = TitleContainer.OpenStream("stone_drag.wav"))
            {
                FrameworkDispatcher.Update();
                effect = SoundEffect.FromStream(stream);
            }

            stacks.Add(DiscStack.One, new Stack<HanoiDisc>());
            stacks.Add(DiscStack.Two, new Stack<HanoiDisc>());
            stacks.Add(DiscStack.Three, new Stack<HanoiDisc>());

            stackColumns.Add(DiscStack.One, leftSpacing);
            stackColumns.Add(DiscStack.Two, virtualColumnWidth + leftSpacing - 4);
            stackColumns.Add(DiscStack.Three, (virtualColumnWidth * 2) + leftSpacing - 4);
            BuildHighScores();

            TimerCallback tcb = Timer_Tick;
            timer = new Timer(tcb, null, Timeout.Infinite, Timeout.Infinite);
            winCount = level + 2;
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

        public void Reset()
        {
            timer.Change(Timeout.Infinite, Timeout.Infinite);

            moves = 0;
            seconds = 0;

            stacks.Clear();
            stackRows.Clear();
            stackColumns.Clear();
            winCount = 0;
            Initialize();
        }

        public void Start()
        {
            Reset();

            if (isReLoaded)
            {
                LoadState();
            }
            else
            {
                List<HanoiDisc> col1 = new List<HanoiDisc>();
                BuildStack(col1, DiscStack.One, Level + 1, false);
                ApplyStack(col1, DiscStack.One);
            }

            timer.Change(0, 1000);
        }

        private static void LoadDiscData(List<HanoiDisc> col1, List<SaveDiscData> listSaveDiscData)
        {
            for (int i = 0; i <= listSaveDiscData.Count - 1; i++)
            {
                SaveDiscData discData = listSaveDiscData[i];

                HanoiDisc hanoiDisc = new HanoiDisc();
                hanoiDisc.OriginalLeft = discData.OriginalLeft;
                hanoiDisc.OriginalTop = discData.OriginalTop;
                hanoiDisc.SetValue(Canvas.LeftProperty, discData.Left);
                hanoiDisc.SetValue(Canvas.TopProperty, discData.Top);
                hanoiDisc.Size = discData.Size;
                hanoiDisc.DiscStack = discData.DiscStack;

                col1.Add(hanoiDisc);
            }
        }

        private void ApplyStack(List<HanoiDisc> discs, DiscStack discStack)
        {
            for (int i = 0; i <= discs.Count - 1; i++)
            {
                stacks[discStack].Push(discs[i]);
            }
        }

        private void BuildStack(List<HanoiDisc> discs, DiscStack discStack, int count, bool reload)
        {
            for (int i = 0; i <= count; i++)
            {

                HanoiDisc hanoiDisc;
                if (!reload)
                {
                    hanoiDisc = new HanoiDisc();
                    discs.Add(hanoiDisc);
                }
                else
                {
                    hanoiDisc = discs[i];
                }

                double left = ((double)(hanoiDisc.GetValue(Canvas.LeftProperty)) == 0 ? leftSpacing : (double)hanoiDisc.GetValue(Canvas.LeftProperty));
                double top = ((double)(hanoiDisc.GetValue(Canvas.TopProperty)) == 0 ? topStart : (double)hanoiDisc.GetValue(Canvas.TopProperty));

                hanoiDisc.SetValue(Grid.ColumnProperty, 0);
                hanoiDisc.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                hanoiDisc.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                hanoiDisc.CacheMode = new BitmapCache();

                if (hanoiDisc.Size == 0)
                    hanoiDisc.Size = i;

                double scalex = (hanoiDisc.Size * .07);
                double scaley = (hanoiDisc.Size * .05);

                hanoiDisc.RenderTransform = new ScaleTransform() { ScaleX = 1 - scalex, ScaleY = 1 - scaley };
                hanoiDisc.ScaleX = scalex;
                hanoiDisc.ScaleY = scaley;

                if (i > 0 && !reload)
                {
                    HanoiDisc previousDisc = discs[i - 1];
                    top = (double)previousDisc.GetValue(Canvas.TopProperty) - (topSpacing - (topSpacing * scaley));
                    left = leftSpacing + ((double)previousDisc.LayoutRoot.Width * scalex) / 2;
                }

                hanoiDisc.Color = hanoiDisc.Size % 2 > 0 ? "Gold" : "Grey";
                hanoiDisc.SetValue(Canvas.LeftProperty, left);
                hanoiDisc.SetValue(Canvas.TopProperty, top);
                hanoiDisc.SetValue(Canvas.ZIndexProperty, 100 + hanoiDisc.Size);
                hanoiDisc.DiscStack = discStack;


                if (!stackRows.ContainsKey(hanoiDisc.Size))
                    stackRows.Add(hanoiDisc.Size, top);
            }
        }

        private void BuildHighScores()
        {
            if (highScores.Count == 0)
            {
                for (int i = 1; i <= 10; i++)
                {
                    highScores.Add(new Score(i));
                }
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
            else
            {
                top = (topStart + (topSpacing * disc.ScaleY));
            }

            double left = stackColumns[toStack] + ((double)disc.LayoutRoot.Width * disc.ScaleX) / 2;
            disc.SetValue(Canvas.TopProperty, top);
            disc.SetValue(Canvas.LeftProperty, left);

            stacks[toStack].Push(disc);
            disc.DiscStack = toStack;

            moves++;
            if (MoveCompleted != null)
                MoveCompleted(this, new MoveCompletedEventArgs(moves));

            CheckForWin();
        }

        private void CheckForWin()
        {
            if (stacks[DiscStack.Three].Count == winCount)
            {
                //want to make sure the ui has the latest and greatest time data...
                if (LevelTimerTick != null)
                    CallTimerTickEvent();

                timer.Change(Timeout.Infinite, Timeout.Infinite);

                Score currentScore = new Score(level, moves, seconds, DateTime.Today);
                if (CheckIfHighScore(currentScore))
                {
                    highScores[level - 1] = currentScore;
                    SaveHighScores();
                }

                if (level == 10)
                {
                    //TODO: Last Level Completed
                }

                level++;
                TimerCallback tcb = BeginReset;
                Timer t = new Timer(tcb, null, 1000, Timeout.Infinite);
            }
        }

        private void CallTimerTickEvent()
        {
            if (LevelTimerTick != null)
                LevelTimerTick(this, new LevelTimerTickEventArgs(seconds));
        }

        private void BeginReset(object obj)
        {
            Reset();

            if (LevelCompleted != null)
            {
                LevelCompleted(this, EventArgs.Empty);
            }
        }

        private bool CheckIfHighScore(Score currentScore)
        {
            Score score = highScores[level - 1];
            if (highScores.Count == 0)
            {
                return true;
            }

            if (currentScore.Level == level)
            {
                if (((currentScore.Moves < score.Moves) && (currentScore.Seconds == score.Seconds)) ||
                    ((currentScore.Moves == score.Moves) && (currentScore.Seconds < score.Seconds)) ||
                    ((currentScore.Moves < score.Moves) && (currentScore.Seconds < score.Seconds)) ||
                    ((score.Moves == 0 && score.Seconds == 0)))
                    return true;
            }

            return false;
        }

        private void SaveHighScores()
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (isf.FileExists(highScoreFileName))
                    isf.DeleteFile(highScoreFileName);

                using (var stream = isf.OpenFile(highScoreFileName, System.IO.FileMode.CreateNew))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<Score>));
                    serializer.Serialize(stream, highScores);
                }
            }
        }

        public void LoadHighScores()
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                //isf.DeleteFile(highScoreFileName);
                if (isf.FileExists(highScoreFileName))
                {
                    using (var stream = isf.OpenFile(highScoreFileName, System.IO.FileMode.Open))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(List<Score>));
                        highScores = (List<Score>)serializer.Deserialize(stream);
                    }
                }
            }
        }

        private bool IsValidMove(HanoiDisc disc, DiscStack toStack)
        {
            if ((stacks[toStack].Count > 0 && stacks[toStack].Peek().Size > disc.Size) || disc.DiscStack == toStack)
            {
                disc.SetValue(Canvas.TopProperty, disc.OriginalTop);
                disc.SetValue(Canvas.LeftProperty, disc.OriginalLeft);
                VibrateController.Default.Start(TimeSpan.FromSeconds(.5));
                return false;
            }


            effect.Play(0.75f, 0f, 0f);

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
            stack = GetDiscStackDestination(stack, left);
            MoveDiscToStack(current, stack);
        }

        private static DiscStack GetDiscStackDestination(DiscStack stack, double left)
        {
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
            return stack;
        }

        public ReadOnlyCollection<HanoiDisc> GetDiscsInStack(DiscStack stack)
        {
            return new ReadOnlyCollection<HanoiDisc>(stacks[stack].ToArray());
        }

        public int Level
        {
            get { return level; }
            set { level = value; }
        }

        public int Moves
        {
            get { return moves; }
            set { moves = value; }
        }

        private void Timer_Tick(object obj)
        {
            CallTimerTickEvent();
            seconds++;
        }

        public List<Score> HighScores
        {
            get
            {
                LoadHighScores();
                return highScores;
            }
        }

        internal void SaveState()
        {
            SaveGame saveGame = App.GameData.SaveGame;
            saveGame.Level = level;
            saveGame.Seconds = seconds;
            saveGame.Moves = moves;

            saveGame.StackOneCount = stacks[DiscStack.One].Count;
            saveGame.StackTwoCount = stacks[DiscStack.Two].Count;
            saveGame.StackThreeCount = stacks[DiscStack.Three].Count;

            SaveStackData(stacks[DiscStack.One], saveGame.SaveDiscDataOne);
            SaveStackData(stacks[DiscStack.Two], saveGame.SaveDiscDataTwo);
            SaveStackData(stacks[DiscStack.Three], saveGame.SaveDiscDataThree);

            App.GameData.SaveGame = saveGame;
        }

        private void SaveStackData(Stack<HanoiDisc> stack, List<SaveDiscData> saveDiscData)
        {
            saveDiscData.Clear();
            HanoiDisc[] discs = stack.ToArray();

            for (int i = discs.Length - 1; i >= 0; i--)
            {
                HanoiDisc currDisc = discs[i];
                SaveDiscData discData = new SaveDiscData();
                discData.Left = (double)currDisc.GetValue(Canvas.LeftProperty);
                discData.Top = (double)currDisc.GetValue(Canvas.TopProperty);
                discData.OriginalLeft = currDisc.OriginalLeft;
                discData.OriginalTop = currDisc.OriginalTop;
                discData.Size = currDisc.Size;
                discData.DiscStack = currDisc.DiscStack;
                saveDiscData.Add(discData);
            }
        }

        internal void LoadStateData()
        {
            isReLoaded = true;
        }

        private void LoadState()
        {
            isReLoaded = false;
            List<HanoiDisc> col1 = new List<HanoiDisc>();
            List<HanoiDisc> col2 = new List<HanoiDisc>();
            List<HanoiDisc> col3 = new List<HanoiDisc>();

            level = App.GameData.SaveGame.Level;
            winCount = level + 2;
            seconds = App.GameData.SaveGame.Seconds;
            moves = App.GameData.SaveGame.Moves;

            LoadDiscData(col1, App.GameData.SaveGame.SaveDiscDataOne);
            BuildStack(col1, DiscStack.One, App.GameData.SaveGame.StackOneCount - 1, true);
            ApplyStack(col1, DiscStack.One);

            LoadDiscData(col2, App.GameData.SaveGame.SaveDiscDataTwo);
            BuildStack(col2, DiscStack.Two, App.GameData.SaveGame.StackTwoCount - 1, true);
            ApplyStack(col2, DiscStack.Two);

            LoadDiscData(col3, App.GameData.SaveGame.SaveDiscDataThree);
            BuildStack(col3, DiscStack.Three, App.GameData.SaveGame.StackThreeCount - 1, true);
            ApplyStack(col3, DiscStack.Three);
        }

    }
}
