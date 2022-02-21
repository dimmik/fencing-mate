using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FencMate
{
    public class GameConfiguration
    {
        public GameType GameType { get; set; } = GameType.AbsoluteScoreLimit;
        public int ScoreLimit { get; set; } = 5;
        public TimeSpan TimeLimit { get; set; } = TimeSpan.FromMinutes(5);
        public (bool finished, PlayerPosition? winner) IsFinished(FencingGame game)
        {
            var gameTime = game.DateTimeStarted;
            var now = DateTimeOffset.Now;
            if (GameType == GameType.NoLimit)
            {
                if ((now - gameTime) < TimeLimit) // not yet
                {
                    return (false, null); // not finished
                } else
                {
                    var rEvents = game.Events.Where(e => e.Player == PlayerPosition.Right);
                    var lEvents = game.Events.Where(e => e.Player == PlayerPosition.Left);
                    PlayerPosition? p = CalculateWinner(rEvents, lEvents);
                    return (true, p);
                }
            }
            if (GameType == GameType.DiffScoreLimit || GameType == GameType.AbsoluteScoreLimit)
            {
                var rEvents = game.Events.Where(e => e.Player == PlayerPosition.Right);
                var lEvents = game.Events.Where(e => e.Player == PlayerPosition.Left);
                if ((now - gameTime) >= TimeLimit) // by time
                {
                    PlayerPosition? p = CalculateWinner(rEvents, lEvents);
                    return (true, p);
                }
                // absolute score
                if (GameType == GameType.AbsoluteScoreLimit)
                {
                    if (rEvents.Count() >= ScoreLimit || lEvents.Count() >= ScoreLimit)
                    {
                        PlayerPosition? p = CalculateWinner(rEvents, lEvents);
                        return (true, p);
                    } else
                    {
                        return (false, null);
                    }
                }
                if (GameType == GameType.DiffScoreLimit)
                {
                    if (Math.Abs(rEvents.Count() - lEvents.Count()) >= ScoreLimit)
                    {
                        PlayerPosition? p = CalculateWinner(rEvents, lEvents);
                        return (true, p);
                    }
                    else
                    {
                        return (false, null);
                    }
                }
            }
            return (false, null);

        }

        private static PlayerPosition? CalculateWinner(IEnumerable<FencingTouchEvent> rEvents, IEnumerable<FencingTouchEvent> lEvents)
        {
            return rEvents.Count() > lEvents.Count()
                                    ? PlayerPosition.Right
                                    : rEvents.Count() > lEvents.Count()
                                        ? PlayerPosition.Left
                                        : (PlayerPosition?)null;
        }
    }

    public enum GameType
    {
        NoLimit,
        AbsoluteScoreLimit,
        DiffScoreLimit
    }
}
