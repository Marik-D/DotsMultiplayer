using DotsCore;

namespace DefaultNamespace
{
    public static class StateManager
    {
        public static string PlayerName = "Charlie";

        public static ClientState ClientState;
        public static Player MyPlayer => ClientState.Player1Name == PlayerName ? Player.Red : Player.Blue;
    }
}