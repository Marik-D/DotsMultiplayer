using System;
using System.Collections.Generic;

namespace Backend
{
    public class ServerState
    {
        public Dictionary<Guid, GameState> Games = new Dictionary<Guid, GameState>();

        private SocketSession _waitingPlayer = null;

        public GameState OnPlayerConnected(SocketSession player)
        {
            if (_waitingPlayer == null)
            {
                _waitingPlayer = player;
                return null;
            }

            var game = new GameState
            {
                Id = Guid.NewGuid(),
                Players = new List<SocketSession> {_waitingPlayer, player},
            };
            _waitingPlayer = null;
            foreach (var session in game.Players)
            {
                session.AssignGame(game);
            }
            this.Games[game.Id] = game;

            return game;
        }
    }
}