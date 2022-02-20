using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FencMate
{
    partial class FencingMateField
    {
        private readonly FencingGame fencingGame = new FencingGame();
        private void InitGame()
        {
            // bind mouse
            CatchMouse(this);
            fencingGame.Start();
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
            fencingGame.AddEvent(ev, DebugInfo);
            // redraw
            RightPlayer.Text = $"Righ {fencingGame.Events.Where(e => e.Player == Player.Right).Count()}";
            LeftPlayer.Text = $"Left {fencingGame.Events.Where(e => e.Player == Player.Left).Count()}";
        }
        private void DebugInfo(string msg)
        {
            Action<string> action = msg => { DebugText.Text = string.Join("\r\n", DebugText.Text.Split("\r\n").TakeLast(20).Concat(new[] { $"{msg}" })); };
            if (InvokeRequired)
            {
                Invoke(action, msg);
            } else
            {
                action(msg);
            }
        }
        public override bool PreProcessMessage(ref Message msg)
        {
            return base.PreProcessMessage(ref msg);
        }
    }
}
