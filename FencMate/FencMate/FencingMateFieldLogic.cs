using System;
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
        private bool Sounds = false;
        private SoundPlayer ToucheSound;
        private SoundPlayer ReadySound;
        private SoundPlayer ToucheTouchSound;
        private void InitGame()
        {
            // bind mouse
            CatchMouseRecursively(this);
            Game.OnReadySet = OnReadySet;
            Game.OnToucheSet = OnToucheSet;
            Game.OnTouchFrom = OnTouchFrom;
            Game.OnToucheTouch = OnToucheTouch;
            Game.OnStop = OnStop;

            SetupSounds();

            ReflectSoundsInfo();
            //Game.Start();
        }


        private void SetupSounds()
        {
            ReadySound = new SoundPlayer(@".\Ready.wav");
            ToucheSound = new SoundPlayer(@".\Touche.wav");
            ToucheTouchSound = new SoundPlayer(@".\ToucheTouch.wav");
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
            if (e.KeyCode == Keys.Space)
            {
                ToggleSound();
            }
            if (e.KeyCode == Keys.P)
            {
                if (Game.State == GameState.Stopped)
                {
                    Game.Resume();
                }
                else
                {
                    Game.Pause();
                }
                UpdateViewport();
            }
            if (e.KeyCode == Keys.R)
            {
                if (Game.State == GameState.Stopped)
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
            if (e.Button == MouseButtons.Middle)
            {
                if (Game.State != GameState.Stopped)
                {
                    Game.Stop();
                }
                else
                {
                    Game.Start();
                }
            }
            if (Game.State == GameState.Stopped)
            {
                return;
            }
            if (!(e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)) return; // only r/l buttons below

            Player player = e.Button == MouseButtons.Left ? Player.Left : Player.Right;
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
                var rEvents = Game.Events.Where(e => e.Player == Player.Right);
                var lEvents = Game.Events.Where(e => e.Player == Player.Left);
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
                                .Select((e, idx) => $@"{idx + 1}. {e.DateTime - Game.DateTimeStarted:mm\:ss\.ff} {(e.IsDouble ? "DBL" : "")}")
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
        private void OnTouchFrom(Player p)
        {
            Action a = () =>
            {
                (p == Player.Left ? LeftPlayer : RightPlayer).BackColor = p == Player.Left ? Color.Red : Color.Green;
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
