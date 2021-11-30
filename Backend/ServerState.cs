using System;
using System.Collections.Generic;

namespace Backend
{
    public class ServerState
    {
        public Dictionary<Guid, GameState> Games = new Dictionary<Guid, GameState>();

        private PlayerSession _waitingPlayer = null;

        public GameState OnPlayerConnected(PlayerSession player)
        {
            if (_waitingPlayer == null)
            {
                _waitingPlayer = player;
                return null;
            }

            var game = new GameState
            {
                Id = Guid.NewGuid(),
                Players = new List<PlayerSession> {_waitingPlayer, player},
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