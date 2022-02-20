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
        private readonly FencingGame fencingGame = new FencingGame();
        private SoundPlayer ToucheSound;
        private SoundPlayer ReadySound;
        private SoundPlayer ToucheTouchSound;
        private void InitGame()
        {
            // bind mouse
            CatchMouse(this);
            fencingGame.OnReadySet = OnReadySet;
            fencingGame.OnToucheSet = OnToucheSet;
            fencingGame.OnTouchFrom = OnTouchFrom;
            fencingGame.OnToucheTouch = OnToucheTouch;

            SetupSounds();

            fencingGame.Start();
        }

        private void SetupSounds()
        {
            ReadySound = new SoundPlayer(@".\Ready.wav");
            ToucheSound = new SoundPlayer(@".\Touche.wav");
            ToucheTouchSound = new SoundPlayer(@".\ToucheTouch.wav");
        }

        private void CatchMouse(Control ctl)
        {
            ctl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ToucheMouseDown);
            foreach (Control c in ctl.Controls)
            {
                CatchMouse(c);
            }
        }

        private void ToucheMouseDown(object sender, MouseEventArgs e)
        {
            Player player = e.Button == MouseButtons.Left ? Player.Left : Player.Right;
            var ev = new FencingTouchEvent()
            {
                DateTime = DateTimeOffset.Now,
                Player = player
            };
            fencingGame.AddEvent(ev, (msg) => { } /*DebugInfo*/);
            // redraw
            RightPlayer.Text = $"Right {fencingGame.Events.Where(e => e.Player == Player.Right).Count()}";
            LeftPlayer.Text = $"Left {fencingGame.Events.Where(e => e.Player == Player.Left).Count()}";
        }
        private void DebugInfo(string msg)
        {
            Action<string> action = msg => { GameState.Text = string.Join("\r\n", GameState.Text.Split("\r\n").TakeLast(20).Concat(new[] { $"{msg}" })); };
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
                GameState.Text = "READY";
                Task.Run(() => { ReadySound.Play(); });
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
                GameState.Text = "TOUCHE";
                Task.Run(() => { ToucheSound.Play(); });
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
                Task.Run(() => { ToucheTouchSound.Play(); });
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
