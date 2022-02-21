﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FencMate
{
    partial class FencingMateField
    {
        private readonly FencingGame Game = new FencingGame();
        private readonly GameConfiguration GameConfiguration = new GameConfiguration();
        private bool Sounds = false;
        private SoundPlayer ToucheSound;
        private SoundPlayer ReadySound;
        private SoundPlayer ToucheTouchSound;
        private SoundPlayer FinishedSound;
        private void InitGame()
        {
            var types = Enum.GetValues(typeof(GameType)).Cast<GameType>().Select(e => (object)e).ToArray();
            GameTypeCombobox.Items.Clear();
            GameTypeCombobox.Items.AddRange(types);
            GameTypeCombobox.SelectedIndex = Array.IndexOf(types, GameConfiguration.GameType);
            GameTypeCombobox.SelectedIndexChanged += GameTypeCombobox_SelectedIndexChanged;

            ScoreLimitUpdown.Value = GameConfiguration.ScoreLimit;
            ScoreLimitUpdown.ValueChanged += ScoreLimitUpdown_ValueChanged;

            TimeLimitUpDown.Value = (int)GameConfiguration.TimeLimit.TotalMinutes;
            TimeLimitUpDown.ValueChanged += TimeLimitUpDown_ValueChanged;

            // bind mouse
            CatchMouseRecursively(this);
            Game.OnReadySet = OnReadySet;
            Game.OnToucheSet = OnToucheSet;
            Game.OnTouchFrom = OnTouchFrom;
            Game.OnToucheTouch = OnToucheTouch;
            Game.OnStop = OnStop;
            Game.OnFinished = OnFinished;

            Game.IsFinished = g => {
                var (f, p) = GameConfiguration.IsFinished(g);
                return f;
            };

            SetupSounds();

            ReflectSoundsInfo();

            InitTimer();

            DisplayGameConfig();
        }

        private void TimeLimitUpDown_ValueChanged(object sender, EventArgs e)
        {
            var ctl = sender as NumericUpDown;
            GameConfiguration.TimeLimit = TimeSpan.FromMinutes((int)ctl.Value);
        }

        private void ScoreLimitUpdown_ValueChanged(object sender, EventArgs e)
        {
            var ctl = sender as NumericUpDown;
            GameConfiguration.ScoreLimit = (int)ctl.Value;
        }

        private void GameTypeCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            var i = comboBox.SelectedIndex;
            var gt = comboBox.Items[i] as GameType?;
            if (gt != null)
            {
                GameConfiguration.GameType = gt ?? GameType.AbsoluteScoreLimit;
            }
        }

        private void DisplayGameConfig()
        {
            Action a = () =>
            {
                /*GameConfigurationLabel.Text = @$"
Game Configuration:
Type: {GameConfiguration.GameType}
ScoreLimit: {GameConfiguration.ScoreLimit}
TimeLimit: {GameConfiguration.TimeLimit}
";*/
            };
            if (InvokeRequired)
            {
                Invoke(a);
            } else
            {
                a();
            }
        }

        private void InitTimer()
        {
            System.Threading.Timer t = new System.Threading.Timer((s) => {
                Action a = () => {
                    if (!(Game.State == GameState.Stopped || Game.State == GameState.Finished))
                    {
                        var gStart = Game.DateTimeStarted;
                        var now = DateTimeOffset.Now;
                        var timeLimit = GameConfiguration.TimeLimit;
                        TimeSpan remains = timeLimit - (now - gStart);
                        TimerLabel.Text = $@"{remains:mm\:ss}";
                    }
                    };
                if (InvokeRequired) 
                {
                    Invoke(a);
                } 
                else
                {
                    a();
                }
            }, null, 1000, 1000);
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
        }

        private void CatchMouseRecursively(Control ctl)
        {
            ctl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ToucheMouseDown);
            ctl.KeyDown += new System.Windows.Forms.KeyEventHandler(KeyboardDown);
            foreach (Control c in ctl.Controls)
            {
                CatchMouseRecursively(c);
            }
        }

        private void KeyboardDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            if (e.KeyCode == Keys.Space)
            {
                ToggleSound();
            }
            if (e.KeyCode == Keys.P) // pause
            {
                if (Game.State == GameState.Ready || Game.State == GameState.Touche)
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
            }
            if (e.KeyCode == Keys.R) // reset
            {
                if (Game.State == GameState.Stopped || Game.IsFinished(Game))
                {
                    Game.Start();
                }
                else
                {
                    Game.Stop();
                }
                UpdateViewport();
            }
        }

        private void ToggleSound()
        {
            Sounds = !Sounds;
            ReflectSoundsInfo();
        }

        private void ReflectSoundsInfo()
        {
            Action a = () =>
            {
                SoundsLabel.Text = $"[space to toggle] Sounds: {(Sounds ? "on" : "off")}";
            };
            if (InvokeRequired)
            {
                Invoke(a);
            }
            else
            {
                a();
            }
        }

        private void ToucheMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle) // Restart
            {
                if (Game.State == GameState.Stopped || Game.IsFinished(Game))
                {
                    Game.Start();
                }
                else
                {
                    Game.Stop();
                }
                UpdateViewport();
            }
            if (Game.State != GameState.Ready && Game.State != GameState.Touche && Game.State != GameState.OneTouch)
            {
                return;
            }
            if (!(e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)) return; // only r/l buttons below

            PlayerPosition player = e.Button == MouseButtons.Left ? PlayerPosition.Left : PlayerPosition.Right;
            var ev = new FencingTouchEvent()
            {
                DateTime = DateTimeOffset.Now,
                Player = player
            };
            Game.AddEvent(ev, (msg) => { } /*DebugInfo*/);
            UpdateViewport();
        }

        private void UpdateViewport()
        {
            Action update = () =>
            {
                var rEvents = Game.Events.Where(e => e.Player == PlayerPosition.Right);
                var lEvents = Game.Events.Where(e => e.Player == PlayerPosition.Left);
                // redraw
                RightPlayer.Text = $"Right {rEvents.Count()}";
                LeftPlayer.Text = $"Left {lEvents.Count()}";

                LeftEvents.Text  = "Events\r\n" + Touches(lEvents);
                RightEvents.Text = "Events\r\n" + Touches(rEvents);
            };
            update();
        }

        private string Touches(IEnumerable<FencingTouchEvent> rEvents)
        {
            var s = string.Join("\r\n",
                                rEvents
                                .Select((e, idx) => $@"{idx + 1}. {e.DateTime - Game.DateTimeStarted:mm\:ss\.fff} {(e.IsDouble ? "DBL" : "")}")
                                .Reverse()
                                .Take(15)
                                );
            return s;
        }

        private void DebugInfo(string msg)
        {
            Action<string> action = msg => { GameStateInfo.Text = string.Join("\r\n", GameStateInfo.Text.Split("\r\n").TakeLast(20).Concat(new[] { $"{msg}" })); };
            if (InvokeRequired)
            {
                Invoke(action, msg);
            } else
            {
                action(msg);
            }
        }
        private void OnReadySet()
        {
            Action a = () =>
            {
                LeftPlayer.BackColor = this.BackColor;
                RightPlayer.BackColor = this.BackColor;
                GameStateInfo.Text = "READY";
                Task.Run(() => { if (Sounds) ReadySound.Play(); });
            };
            if (InvokeRequired)
            {
                Invoke(a);
            } else
            {
                a();
            }
        }
        private void OnTouchFrom(PlayerPosition p)
        {
            Action a = () =>
            {
                (p == PlayerPosition.Left ? LeftPlayer : RightPlayer).BackColor = p == PlayerPosition.Left ? Color.Red : Color.Green;
            };
            if (InvokeRequired)
            {
                Invoke(a);
            }
            else
            {
                a();
            }
        }
        private void OnToucheSet()
        {
            Action a = () =>
            {
                GameStateInfo.Text = "TOUCHE";
                Task.Run(() => { if (Sounds) ToucheSound.Play(); });
                var (finished, winner) = GameConfiguration.IsFinished(Game);
                if (finished) Game.Stop();
            };
            if (InvokeRequired)
            {
                Invoke(a);
            }
            else
            {
                a();
            }
        }
        private void OnFinished()
        {
            Action a = () =>
            {
                LeftPlayer.BackColor = this.BackColor;
                RightPlayer.BackColor = this.BackColor;
                var (f, winner) = GameConfiguration.IsFinished(Game);
                GameStateInfo.Text = $"Finished\r\nW: {(winner == null ? "No" : winner == PlayerPosition.Left ? "Left" : "Right")}";
                Task.Run(() => { if (Sounds) FinishedSound.Play(); });
            };
            if (InvokeRequired)
            {
                Invoke(a);
            }
            else
            {
                a();
            }
        }
        private void OnToucheTouch()
        {
            Action a = () =>
            {
                Task.Run(() => { if (Sounds) ToucheTouchSound.Play(); });
            };
            if (InvokeRequired)
            {
                Invoke(a);
            }
            else
            {
                a();
            }
        }
        private void OnStop()
        {
            Action a = () =>
            {
                GameStateInfo.Text = "Stopped";
                UpdateViewport();
            };
            if (InvokeRequired)
            {
                Invoke(a);
            }
            else
            {
                a();
            }
        }


    }
}
