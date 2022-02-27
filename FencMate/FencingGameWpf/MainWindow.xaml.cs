﻿using System;
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

        private GameConfiguration GameConfiguration;
        private IConfiguration configuration;
        public MainWindow()
        {
            InitializeComponent();
            // init config
            BuildConfig();
            // init game
            InitGame();
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
        }

        private void OnStop()
        {
        }

        private void OnToucheTouch()
        {
        }

        private void OnTouchFrom(PlayerPosition p)
        {
            Action a = () =>
            {
                (p == PlayerPosition.Left ? LeftPlayerLabel : RightPlayerLabel).Background = p == PlayerPosition.Left ? Brushes.Red : Brushes.Green;
            };
            this.Dispatcher.Invoke(a);
        }
        private void OnToucheSet()
        {
            Action a = () =>
            {
                //GameStateInfo.Text = "TOUCHE";
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

                //var ts = touch == PlayerPosition.Left ? LeftSound : touch == PlayerPosition.Right ? RightSound : ToucheSound;
                //if (Sounds) ts.PlaySync();
                var (finished, winner) = GameConfiguration.IsFinished(Game);
                if (finished) Game.Finish();
                UpdateViewport();
            };
            Dispatcher.Invoke(a);
        }

        //private Label 

        private void OnReadySet()
        {
            Action a = () =>
            {
                LeftPlayerLabel.Background = this.Background;
                RightPlayerLabel.Background = this.Background;
                //GameStateInfo.Text = "READY";
                //SetEnabledGameControls(this, false);
                //Task.Run(() => { if (Sounds) ReadySound.Play(); });
            };
            this.Dispatcher.Invoke(a);
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
                                .Select((e, idx) => $@"{idx + 1}. {e.DateTime - Game.DateTimeStarted:mm\:ss\.fff} {(e.IsDouble ? "DBL" : "")}")
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

            //RegisterHotKey(handle, HOTKEY_ID, 0x0, 173); // hotkey vol onoff
            RegisterHotKey(handle, HOTKEY_ID, 0x0, 174); // hotkey vol down
            RegisterHotKey(handle, HOTKEY_ID, 0x0, 175); // hotkey vol up
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    switch (wParam.ToInt32())
                    {
                        case HOTKEY_ID:
                            int vkey = (((int)lParam >> 16) & 0xFFFF);
                            if (vkey == 174 || vkey == 175) // vol down or up
                            {
                                PlayerPosition p = vkey == 174 ? PlayerPosition.Left : PlayerPosition.Right;
                                ProcessGameEvent(p);
                            }
                            handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }


        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            //ProcessGameEvent(PlayerPosition.Left);
            //if (e.)
        }
        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            //ProcessGameEvent(PlayerPosition.Right);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

        }

        private void ProcessGameEvent(PlayerPosition player)
        {
            if (Game.State.IsInprogress())
            {
                var ev = new FencingTouchEvent()
                {
                    DateTime = DateTimeOffset.Now,
                    Player = player
                };
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
            ProcessGameEvent(p);
        }

    }
}
