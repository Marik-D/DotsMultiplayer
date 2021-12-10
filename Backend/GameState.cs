using System;
using System.Collections.Generic;
using DotsCore;

namespace Backend
{
    public class GameState
    {
        public Guid Id;
        
        public BoardState BoardState = new BoardState(15, 15);
        
        public List<PlayerSession> Players = new List<PlayerSession>();

        public void MakeMove(Move move)
        {
            BoardState.Place(move.Row, move.Col);

            foreach (var session in Players)
            {
                session.UpdateBoardState(BoardState);
            }
        }

        public void FinishGame(Player player)
        {
            Console.WriteLine($"Game[{Id}] {player} has finished");
            if (player == Player.Red)
            {
                BoardState.RedFinished = true;
            }
            else
            {
                BoardState.BlueFinished = true;
            }
            
            foreach (var session in Players)
            {
                session.UpdateBoardState(BoardState);
            }

            if (BoardState.IsGameOver)
            {
                Console.WriteLine($"Game[{Id}] Game over. Score: {BoardState.RedScore} - {BoardState.BlueScore}");
                foreach (var session in Players)
                {
                    session.GameOver(BoardState.RedScore > BoardState.BlueScore ? Player.Red : Player.Blue);
                }
            }
        }
    }
}