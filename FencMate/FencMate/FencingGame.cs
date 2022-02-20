using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace FencMate
{
    public class FencingGame
    {
        private readonly int SameDiffInMs = 40; // 0.04 s
        private readonly int ReadyInMs = 3000; // 3 s
        private List<FencingTouchEvent> events { get; } = new List<FencingTouchEvent>();
        public IEnumerable<FencingTouchEvent> Events => events;
        public GameState State { get; private set; } = GameState.Stopped;
        public void Start()
        {
            State = GameState.Ready;
        }
        private readonly object evLock = new object();

        public void AddEvent(FencingTouchEvent e, Action<string> log)
        {
            lock (evLock)
            {
                if (log == null) log = ((m) => { });
                if (State == GameState.Ready)
                {
                    log($"Ready. Touch from {e.Player}");
                    events.Add(e);
                    SetOneTouch(log);
                    return;
                } else  if (State == GameState.OneTouch)
                {
                    log($"One Touch. Touch from {e.Player}");
                    var lastEvent = events.Count > 0 ? events[events.Count - 1] : null;
                    if (lastEvent?.Player != e.Player)
                    {
                        log($"Double Touch");
                        events.Add(e);
                    }
                    SetTouche(log);
                    return;
                } else if (State == GameState.Touche)
                {
                    log("Touche. Wait please");
                } else
                {
                    log($"Hmm. State is {State}");
                }
            }
        }
        private void SetTouche(Action<string> log)
        {
            ToucheTimer?.Change(Timeout.Infinite, Timeout.Infinite);
            if (State != GameState.Touche)
            {
                log($"Set Touche");
                State = GameState.Touche;
                // 3s after touche new touche
                ReadyTimer = new Timer((s) => { SetReady(log); }, null, ReadyInMs, Timeout.Infinite);
            }
        }
        private void SetReady(Action<string> log)
        {
            ReadyTimer?.Change(Timeout.Infinite, Timeout.Infinite);
            log($"Set Ready");
            State = GameState.Ready;
        }
        private void SetOneTouch(Action<string> log)
        {
            log($"Set OneTouch");
            State = GameState.OneTouch;
            ToucheTimer = new Timer((s) => { SetTouche(log); }, null, SameDiffInMs, Timeout.Infinite);
        }
        private Timer OneTouchTimer;
        private Timer ToucheTimer;
        private Timer ReadyTimer;

    }

    public enum GameState
    {
        Stopped,
        Ready,
        OneTouch,
        Touche
    }

    public class FencingTouchEvent
    {
        public Player Player;
        public DateTimeOffset DateTime;
    }
    public enum Player
    {
        Left, Right
    }
}
