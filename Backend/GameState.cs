using System;
using System.Collections.Generic;
using DotsCore;

namespace Backend
{
    public class GameState
    {
        public Guid Id;
        
        public BoardState BoardState = new BoardState(15, 15);
        
        public List<SocketSession> Players = new List<SocketSession>();

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