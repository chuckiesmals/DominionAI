using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DominionAI
{
    class Simulator
    {
        enum Phase
        {
            Action,
            Buy
        }

        List<Player> _players;
        List<Card> _kingdomCards;
        int _numberOfPlayers = 2;
        int _currentPlayer;
        Phase _currentPhase;

        public Simulator(int numPlayers)
        {
            _numberOfPlayers = numPlayers;
            _currentPlayer = 0;
            _currentPhase = Phase.Action;

            _players = new List<Player>();
            _kingdomCards = new List<Card>();

            for (int i = 0; i < _numberOfPlayers; i++)
            {
                _players.Add(new Player());
            }
        }

        public void Update()
        {
            Console.WriteLine("Player " + _currentPlayer.ToString() + " update.");
            _players[_currentPlayer].Update(this);
        }

        public void NextPlayerTurn()
        {
            _currentPlayer++;
            if (_currentPlayer >= _numberOfPlayers)
            {
                _currentPlayer = 0;
            }
        }
    }
}
