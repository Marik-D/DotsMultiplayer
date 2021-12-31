namespace DotsCore
{
    public class ClientState
    {
        public enum StateEnum
        {
            None,
            Matchmaking,
            Playing,
            GameOver,
        }

        public StateEnum State;

        public string Player1Name;
        public string Player2Name;

        public Player Winner;
    }
}