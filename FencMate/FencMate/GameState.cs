namespace FencMate
{
    public enum GameState
    {
        Stopped,
        Ready,
        OneTouch,
        Touche,
        Finished,
        //Check
    }
    public static class GameStateLogic
    {
        public static bool IsStale(this GameState s)
        {
            return (
                s == GameState.Finished
                || s == GameState.Stopped
                );
        }
        public static bool IsInprogress(this GameState s)
        {
            return (
                s == GameState.Ready
                || s == GameState.Touche
                || s == GameState.OneTouch
                );
        }
    }
}
