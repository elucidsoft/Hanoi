using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Devices;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using ProtoBuf;

namespace Hanoi
{
    public enum DiscStack { One, Two, Three };

    public class GameManager : IDisposable
    {
        private bool disposed = false;
        private static GameManager instance;

        public event EventHandler LevelCompleted;
        public event EventHandler<MoveCompletedEventArgs> MoveCompleted;
        public event EventHandler<LevelTimerTickEventArgs> LevelTimerTick;
        public event EventHandler<HighScoreEventArgs> HighScore;
        public event EventHandler<EventArgs> LightVersionCompleted;

        Dictionary<DiscStack, Stack<HanoiDisc>> stacks = new Dictionary<DiscStack, Stack<HanoiDisc>>();
        Dictionary<double, double> stackRows = new Dictionary<double, double>();
        Dictionary<DiscStack, double> stackColumns = new Dictionary<DiscStack, double>();
        PhoneApplicationFrame phoneAppFrame = (Application.Current.RootVisual as PhoneApplicationFrame);
        List<Score> highScores = new List<Score>();

        private const string HighScoreFileName = "highscores.bin";

        private const double virtualColumnWidth = 266;
        private const double virtualContainerCount = 3;
        private const double topStart = 360;
        private const double topSpacing = 42;
        private const double leftSpacing = 45;
        private int level = 1;
        private int moves = 0;
        private int seconds = 0;
        private Timer timer;
        private Timer resetDelayTimer;
        private SoundEffect effect = null;

        public int winCount = 0;
        private bool isReLoaded = false;
        private bool isDirty = false;

        #region Initialization/Constructor/Begin/Reset

        private GameManager()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (App.GameData.GameSettings.PlaySounds)
            {
                FrameworkDispatcher.Update();

                using (Stream stream = TitleContainer.OpenStream("stone_drag.wav"))
                {
                    effect = SoundEffect.FromStream(stream);
                }
            }

            stacks.Add(DiscStack.One, new Stack<HanoiDisc>());
            stacks.Add(DiscStack.Two, new Stack<HanoiDisc>());
            stacks.Add(DiscStack.Three, new Stack<HanoiDisc>());

            stackColumns.Add(DiscStack.One, leftSpacing);
            stackColumns.Add(DiscStack.Two, virtualColumnWidth + leftSpacing - 16);
            stackColumns.Add(DiscStack.Three, (virtualColumnWidth * 2) + leftSpacing - 38);
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


        private void BeginReset(object obj)
        {
            Reset();

            if (LevelCompleted != null)
            {
                LevelCompleted(this, EventArgs.Empty);
            }
        }

        public void Start(bool delayTimer)
        {
            isDirty = true;
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

            if (!delayTimer)
            {
                timer.Change(0, 1000);
            }
        }

        public void StartTimer()
        {
            timer.Change(0, 1000);
        }

        #endregion

        #region Game Play Methods

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

                double scalex = (hanoiDisc.Size * .058);
                double scaley = (hanoiDisc.Size * .0549);
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

            App.Current.RootVisual.Dispatcher.BeginInvoke(() =>
            {

                double left = stackColumns[toStack] + ((double)disc.LayoutRoot.Width * disc.ScaleX) / 2;
                disc.SetValue(Canvas.TopProperty, top);
                disc.SetValue(Canvas.LeftProperty, left);

            });

            stacks[toStack].Push(disc);
            disc.DiscStack = toStack;

            if (MoveCompleted != null)
                MoveCompleted(this, new MoveCompletedEventArgs(moves));

            CheckForWin();
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

        private bool IsValidMove(HanoiDisc disc, DiscStack toStack)
        {
            if ((stacks[toStack].Count > 0 && stacks[toStack].Peek().Size > disc.Size))
            {
                disc.SetValue(Canvas.TopProperty, disc.OriginalTop);
                disc.SetValue(Canvas.LeftProperty, disc.OriginalLeft);

                if (App.GameData.GameSettings.VibrateOnInvalidMove)
                {
                    VibrateController.Default.Start(TimeSpan.FromSeconds(.25));
                }
                return false;
            }

            if (disc.DiscStack != toStack)
            {
                moves++;
            }

            if (App.GameData.GameSettings.PlaySounds)
            {
                float volume = Convert.ToSingle(App.GameData.GameSettings.SoundVolume / 100);
                effect.Play(volume, 0f, 0f);
            }

            return true;
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

                if (App.IsLiteVersion && level == 6)
                {
                    if (LightVersionCompleted != null)
                        LightVersionCompleted(this, new EventArgs());

                    Reset();
                    return;
                }

                if (level == 7)
                {
                    level = 0;
                }

                level++;
                TimerCallback tcb = BeginReset;

                if (resetDelayTimer != null)
                    resetDelayTimer.Dispose();

                resetDelayTimer = new Timer(tcb, null, 1000, Timeout.Infinite);
            }
        }

        #endregion

        #region Event Handler Methods

        private void CallTimerTickEvent()
        {
            if (LevelTimerTick != null)
                LevelTimerTick(this, new LevelTimerTickEventArgs(seconds));
        }

        #endregion

        #region HighScore Methods

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
                {
                    if (HighScore != null && !(score.Moves == 0 && score.Seconds == 0))
                        HighScore(this, new HighScoreEventArgs(currentScore));

                    return true;
                }
            }

            return false;
        }

        private void BuildHighScores()
        {
            if (highScores.Count == 0)
            {
                for (int i = 1; i <= 12; i++)
                {
                    highScores.Add(new Score(i));
                }
            }
        }

        private List<Score> ReturnHighScores(List<Score> scores)
        {
            if(scores != null && scores.Count == 10)
            {
                BuildHighScores();

                for (int i = 0; i <= scores.Count - 1; i++)
                {
                    highScores[i] = scores[i];
                }

                scores.Add(new Score(11));
                scores.Add(new Score(12));
            }

            return scores;
        }

        private void SaveHighScores()
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (isf.FileExists(HighScoreFileName))
                    isf.DeleteFile(HighScoreFileName);

                using (var stream = isf.OpenFile(HighScoreFileName, System.IO.FileMode.CreateNew))
                {
                    Serializer.Serialize<List<Score>>(stream, highScores);
                }
            }
        }

        public void LoadHighScores()
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                //isf.DeleteFile(HighScoreFileName);
                if (isf.FileExists(HighScoreFileName))
                {
                    using (var stream = isf.OpenFile(HighScoreFileName, System.IO.FileMode.Open))
                    {
                        highScores = ReturnHighScores(Serializer.Deserialize<List<Score>>(stream));
                        //highScores = Serializer.Deserialize<List<Score>>(stream);
                    }
                }
            }
        }

        public void ClearHighScores()
        {
            highScores.Clear();
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                isf.DeleteFile(HighScoreFileName);
            }
            BuildHighScores();
        }

        #endregion

        #region Properties


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
                return highScores;
            }
        }

        #endregion

        #region State Saving/Loading

        internal void SaveState()
        {
            if (isDirty == false)
                return;

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
            LoadHighScores();
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
                if (effect != null)
                    effect.Dispose();

                if (resetDelayTimer != null)
                    resetDelayTimer.Dispose();

                if (timer != null)
                    timer.Dispose();
            }
            disposed = true;
        }

        ~GameManager()
        {
            Dispose(false);
        }

        #endregion
    }
}
