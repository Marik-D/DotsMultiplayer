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
    }
}