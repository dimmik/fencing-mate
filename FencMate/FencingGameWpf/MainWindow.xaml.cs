using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Windows.Interop;
using System.Runtime.InteropServices;
using FencingGame;
using Microsoft.Extensions.Configuration;
using System.Media;

namespace FencingGameWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Game Game;

        public bool CheckMode { get; private set; }
        public bool Sounds { get; private set; }
        public SoundPlayer ReadySound { get; private set; }
        public SoundPlayer ToucheSound { get; private set; }
        public SoundPlayer ToucheTouchSound { get; private set; }
        public SoundPlayer FinishedSound { get; private set; }
        public SoundPlayer FinishedLSound { get; private set; }
        public SoundPlayer FinishedRSound { get; private set; }
        public SoundPlayer FinishedDSound { get; private set; }
        public SoundPlayer LeftSound { get; private set; }
        public SoundPlayer RightSound { get; private set; }

        private GameConfiguration GameConfiguration = new GameConfiguration(GameType.AbsoluteScoreLimit, 6, TimeSpan.FromMinutes(3));
        private IConfiguration configuration;
        public MainWindow()
        {
            InitializeComponent();
            // init config
            BuildConfig();
            SetupSounds();
            // init game
            InitGame();
            InitConfigDisplay();
            InitTimer();
        }

        private void InitConfigDisplay()
        {
            var types = Enum.GetValues(typeof(GameType)).Cast<GameType>().Select(e => (object)e).ToArray();
            GameTypeComboBox.Items.Clear();
            foreach (var t in types)
            {
                GameTypeComboBox.Items.Add(t);
            }
            GameTypeComboBox.SelectedIndex = Array.IndexOf(types, GameConfiguration.GameType);

            ScoreLimitSlider.Value = GameConfiguration.ScoreLimit;
            TimeLimitSlider.Value = GameConfiguration.TimeLimit.TotalMinutes;
            //ScoreLimitSlider.
        }

        private void SetupSounds()
        {
            ReadySound = new SoundPlayer(@".\Ready.wav");
            ReadySound.Load();
            ToucheSound = new SoundPlayer(@".\Touche.wav");
            ToucheSound.Load();
            ToucheTouchSound = new SoundPlayer(@".\ToucheTouch.wav");
            ToucheTouchSound.Load();
            FinishedSound = new SoundPlayer(@".\Finished.wav");
            FinishedSound.Load();

            FinishedLSound = new SoundPlayer(@".\FinishedLeftWon.wav");
            FinishedLSound.Load();
            FinishedRSound = new SoundPlayer(@".\FinishedRightWon.wav");
            FinishedRSound.Load();
            FinishedDSound = new SoundPlayer(@".\FinishedDraw.wav");
            FinishedDSound.Load();

            LeftSound = new SoundPlayer(@".\LeftTouch.wav");
            LeftSound.Load();
            RightSound = new SoundPlayer(@".\RightTouch.wav");
            RightSound.Load();
        }


        private System.Threading.Timer timerClock;
        private void InitTimer()
        {
            timerClock = new System.Threading.Timer((s) => {
                Action a = () => {
                    if (!(Game.State.IsStale()))
                    {
                        var gStart = Game.DateTimeStarted;
                        var now = DateTimeOffset.Now;
                        var timeLimit = GameConfiguration.TimeLimit;
                        TimeSpan remains = timeLimit - (now - gStart);
                        GameTimer.Content = $@"{remains:mm\:ss}";
                        if (Game.IsFinished(Game)) Game.Finish();
                    }
                };
                DispatcherInvoke(a);
            }, null, 1000, 1000);
        }

        private void InitGame()
        {
            Game = new Game(
                            sameDiffInMs: configuration.GetValue("SameDiffInMs", 40),
                            readyInMsFrom: configuration.GetValue("ReadyInMsFrom", 1400),
                            readyInMsTo: configuration.GetValue("ReadyInMsTo", 2500)
                            );
            CheckMode = configuration.GetValue("DefaultCheckMode", false);
            Sounds = configuration.GetValue("DefaultSoundsOn", false);

            GameConfiguration = new GameConfiguration(
                gameType: configuration.GetValue("GameType", GameType.AbsoluteScoreLimit),
                scoreLimit: configuration.GetValue("ScoreLimit", 10),
                timeLimit: TimeSpan.FromSeconds(configuration.GetValue("TimeLimitInS", 4 * 60))
                );

            Game.OnReadySet = OnReadySet;
            Game.OnToucheSet = OnToucheSet;
            Game.OnTouchFrom = OnTouchFrom;
            Game.OnToucheTouch = OnToucheTouch;
            Game.OnStop = OnStop;
            Game.OnFinished = OnFinished;

            Game.IsFinished = g =>
            {
                var (f, p) = GameConfiguration.IsFinished(g);
                return f;
            };

            if (configuration.GetValue("GameStartedOnStartup", false))
            {
                Game.Start();
            }
        }

        private void OnFinished()
        {
            Action a = () =>
            {
                var (f, winner) = GameConfiguration.IsFinished(Game);
                LeftPlayerLabel.Background = winner == PlayerPosition.Left ? Brushes.Red : this.Background;
                RightPlayerLabel.Background = winner == PlayerPosition.Right ? Brushes.Green : this.Background;
                GameStateInfo.Content = $"Finished\r\nW: {(winner == null ? "No" : winner == PlayerPosition.Left ? "Left" : "Right")}";
                SetEnabledGameControls(this, true);
                var fs = winner == PlayerPosition.Left ? FinishedLSound : winner == PlayerPosition.Right ? FinishedRSound : FinishedDSound;
                if (Sounds) fs.Play();
            };
            DispatcherInvoke(a);
        }

        private void OnStop()
        {
            Action a = () =>
            {
                GameStateInfo.Content = "Stopped";
                SetEnabledGameControls(this, true);

                UpdateViewport();
            };
            DispatcherInvoke(a);
        }

        private void OnToucheTouch()
        {
            // play touche touch aound
        }

        private void OnTouchFrom(PlayerPosition p)
        {
            Action a = () =>
            {
                (p == PlayerPosition.Left ? LeftPlayerLabel : RightPlayerLabel).Background = p == PlayerPosition.Left ? Brushes.Red : Brushes.Green;
            };
            DispatcherInvoke(a);
        }
        private void OnToucheSet()
        {
            Action a = () =>
            {
                GameStateInfo.Content = "TOUCHE";
                PlayerPosition? touch = null;
                var twoLastTouches = Game.Events.TakeLast(2);
                if (twoLastTouches.Count() < 2)
                {
                    touch = twoLastTouches.FirstOrDefault().Player;
                }
                else
                {
                    var last = twoLastTouches.Last();
                    var first = twoLastTouches.First();
                    var maxDiff = TimeSpan.FromMilliseconds(Game.SameDiffInMs);
                    if (last.DateTime - first.DateTime <= maxDiff) // both
                    {
                        touch = null;
                    }
                    else
                    {
                        touch = last.Player;
                    }
                }

                var ts = touch == PlayerPosition.Left ? LeftSound : touch == PlayerPosition.Right ? RightSound : ToucheSound;
                if (Sounds) ts.PlaySync();
                var (finished, winner) = GameConfiguration.IsFinished(Game);
                if (finished) Game.Finish();
                UpdateViewport();
            };
            DispatcherInvoke(a);
        }

        //private Label 

        private void OnReadySet()
        {
            Action a = () =>
            {
                LeftPlayerLabel.Background = this.Background;
                RightPlayerLabel.Background = this.Background;
                GameStateInfo.Content = "READY";
                SetEnabledGameControls(this, false);
                if (Sounds) ReadySound.Play();
            };
            DispatcherInvoke(a);
        }

        private void DispatcherInvoke(Action a)
        {
            try
            {
                Dispatcher.Invoke(a);
            } catch
            {
                // bad luck.
            }
        }

        private void UpdateViewport()
        {
            Action update = () =>
            {
                var rEvents = Game.Events.Where(e => e.Player == PlayerPosition.Right);
                var lEvents = Game.Events.Where(e => e.Player == PlayerPosition.Left);
                // redraw
                RightPlayerLabel.Content = $"Right {rEvents.Count()}";
                LeftPlayerLabel.Content = $"Left {lEvents.Count()}";

                LeftEventsLabel.Content = "Events\r\n" + TouchesAsString(lEvents);
                RightEventsLabel.Content = "Events\r\n" + TouchesAsString(rEvents);
            };
            update();
        }
        private string TouchesAsString(IEnumerable<FencingTouchEvent> rEvents)
        {
            var s = string.Join("\r\n",
                                rEvents
                                .Select((e, idx) => $@"{idx + 1}. {e.DateTime - (Game.DateTimeStarted - Game.PausedTime):mm\:ss\.fff} {(e.IsDouble ? "DBL" : "")}")
                                .Reverse()
                                .Take(15)
                                );
            return s;
        }
        private void BuildConfig()
        {
            // Build configuration
            configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true)
                .Build();
        }


        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        private HwndSource source;
        private const int HOTKEY_ID = 9000;



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IntPtr handle = new WindowInteropHelper(this).Handle;
            source = HwndSource.FromHwnd(handle);
            source.AddHook(HwndHook);

            RegisterHotKey(handle, HOTKEY_ID, 0x0, 173); // hotkey vol onoff
            RegisterHotKey(handle, HOTKEY_ID, 0x0, 174); // hotkey vol down
            RegisterHotKey(handle, HOTKEY_ID, 0x0, 175); // hotkey vol up
        }
        private List<(DateTimeOffset, PlayerPosition)> hotkeys = new List<(DateTimeOffset, PlayerPosition)>();
        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            //return IntPtr.Zero;
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    switch (wParam.ToInt32())
                    {
                        case HOTKEY_ID:
                            var n = DateTimeOffset.Now;
                            int vkey = (((int)lParam >> 16) & 0xFFFF);
                            var hc = hotkeys.Select(h => $"{h.Item1:mm:ss.ffffff} {h.Item2}").ToArray();
                            if (vkey == 174 || vkey == 175) // vol down or up
                            {
                                PlayerPosition p = vkey == 174 ? PlayerPosition.Left : PlayerPosition.Right;
                                hotkeys.Add((n, p));
                                var ev = new FencingTouchEvent()
                                {
                                    DateTime = n,
                                    Player = p
                                };
                                ProcessGameEvent(ev);
                            }
                            //handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        private void ProcessGameEvent(FencingTouchEvent ev)
        {
            if (Game.State.IsInprogress())
            {
                Game.AddEvent(ev, (msg) => { });
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!(e.ChangedButton == MouseButton.Left || e.ChangedButton == MouseButton.Right))
            {
                return;
            }
            
            PlayerPosition p = e.ChangedButton == MouseButton.Left ? PlayerPosition.Left : PlayerPosition.Right;
            var ev = new FencingTouchEvent()
            {
                DateTime = DateTimeOffset.Now,
                Player = p
            };
            ProcessGameEvent(ev);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.P: // Pause
                    if (Game.State.IsInprogress())
                    {
                        Game.Stop();
                    }
                    else
                    {
                        if (!Game.IsFinished(Game))
                        {
                            Game.Resume();
                        }
                    }
                    UpdateViewport();
                    break;
                case Key.R:
                    if (Game.State.IsInprogress())
                    {
                        Game.Stop();
                    }
                    else
                    {
                        Game.Start();
                    }
                    UpdateViewport();
                    break;
            }

        }

        private void GameType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var chosenType = e.AddedItems[0] as GameType?;
            if (chosenType != null)
                GameConfiguration.GameType = chosenType.Value;
            UpdateGameConfigD();

        }

        private void UpdateGameConfigD()
        {
            DispatcherInvoke(() =>
            {
                try
                {

                    if (ScoreLimitLabel != null) ScoreLimitLabel.Content = $"Up to {GameConfiguration.ScoreLimit}";
                    if (TimeLimitLabel != null) TimeLimitLabel.Content = $"{GameConfiguration.TimeLimit.TotalMinutes} mins";
                }
                catch
                {
                    // hmm. not inited yet?..
                }
            }
            );
        }

        private void SetEnabledGameControls(Control ctl, bool enabled)
        {
            GameTypeComboBox.IsEnabled = enabled;
            ScoreLimitSlider.IsEnabled = enabled;
            TimeLimitSlider.IsEnabled = enabled;
        }

        private void ScoreLimitSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            GameConfiguration.ScoreLimit = (int)(e?.NewValue ?? GameConfiguration.ScoreLimit);
            UpdateGameConfigD();
        }
        private void TimeLimitSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            GameConfiguration.TimeLimit = TimeSpan.FromMinutes((int)(e?.NewValue ?? GameConfiguration.TimeLimit.TotalMinutes));
            UpdateGameConfigD();
        }
    }
}
