using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace FencMate
{
    public class FencingGame
    {
        public readonly int SameDiffInMs = 40; // 0.04 s
        private readonly int ReadyInMsFrom = 1400; // 2 s
        private readonly int ReadyInMsTo = 2500; // 2 s
        private Random r = new Random();
        private List<FencingTouchEvent> events { get; } = new List<FencingTouchEvent>();
        public DateTimeOffset DateTimeStarted { get; private set; } = DateTimeOffset.Now;
        public IEnumerable<FencingTouchEvent> Events => events;
        public GameState State { get; private set; } = GameState.Stopped;
        public void Start()
        {
            events.Clear();
            DateTimeStarted = DateTimeOffset.Now;
            SetReady((m) => { });
        }

        private readonly object evLock = new object();
        public Action<PlayerPosition> OnTouchFrom = (p) => { };
        public Action OnToucheTouch = () => { };
        public Action OnToucheSet = () => { };
        public Action OnReadySet = () => { };
        public Action OnStop = () => { };
        public Action OnFinished = () => { };

        public Func<FencingGame, bool> IsFinished = (g) => false;
        private DateTimeOffset stoppedTime = DateTimeOffset.Now;
        public void Stop()
        {
            State = GameState.Stopped;
            stoppedTime = DateTimeOffset.Now;
            OnStop();
        }
        public void Resume()
        {
            if (!IsFinished(this)) 
            {
                var now = DateTimeOffset.Now;
                var diff = now - stoppedTime;
                DateTimeStarted += diff;
                SetReady((m) => { }); 
            }
        }
        public void Finish()
        {
            State = GameState.Finished;
            OnFinished();
        }

        public void AddEvent(FencingTouchEvent e, Action<string> log)
        {
            lock (evLock)
            {
                if (log == null) log = ((m) => { });
                if (State == GameState.Ready)
                {
                    log($"Ready. Touch from {e.Player}");
                    events.Add(e);
                    OnTouchFrom(e.Player);
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
                        lastEvent.IsDouble = true;
                        e.IsDouble = true;
                        OnTouchFrom(e.Player);
                    }
                    SetTouche(log);
                    return;
                } else if (State == GameState.Touche)
                {
                    log("Touche. Wait please");
                    OnToucheTouch();
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
                OnToucheSet();
                if (!IsFinished(this))
                {
                    // 3s after touche new touche
                    ReadyTimer = new Timer((s) => { SetReady(log); }, null, (r.Next(ReadyInMsFrom, ReadyInMsTo)), Timeout.Infinite);
                } else
                {
                    Finish();
                }
            }
        }


        private void SetReady(Action<string> log)
        {
            ReadyTimer?.Change(Timeout.Infinite, Timeout.Infinite);
            State = GameState.Ready;
            OnReadySet();
        }
        private void SetOneTouch(Action<string> log)
        {
            log($"Set OneTouch");
            State = GameState.OneTouch;
            ToucheTimer = new Timer((s) => { SetTouche(log); }, null, SameDiffInMs, Timeout.Infinite);
        }
        private Timer ToucheTimer;
        private Timer ReadyTimer;

    }
}
