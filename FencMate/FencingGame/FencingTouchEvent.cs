using System;

namespace FencingGame
{
    public class FencingTouchEvent
    {
        public Guid guid = Guid.NewGuid();
        public PlayerPosition Player;
        public DateTimeOffset DateTime;
        public bool IsDouble = false;

        public override string ToString()
        {
            return $"{Player} {DateTime:mm:ss.ffffff} {IsDouble}";
        }
    }
}
