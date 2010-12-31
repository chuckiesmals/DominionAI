using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace DominionAI
{
    public class Simulator
    {
        public List<Player> _players;
        public List<Card.Card> _masterSet;
        List<Card.Card> _kingdomCards;
        public int _numberOfPlayers = 2;
        int _currentPlayer;

        #region Input
        List<Keys> _numKeys;
        Dictionary<Keys, bool> _wasKeyPressed;
        public int _lastNumberPressed = -1;
        #endregion

        public Simulator(int numPlayers)
        {
            _numKeys = new List<Keys>();
            _numKeys.Add(Keys.D0);
            _numKeys.Add(Keys.D1);
            _numKeys.Add(Keys.D2);
            _numKeys.Add(Keys.D3);
            _numKeys.Add(Keys.D4);
            _numKeys.Add(Keys.D5);
            _numKeys.Add(Keys.D6);
            _numKeys.Add(Keys.D7);
            _numKeys.Add(Keys.D8);
            _numKeys.Add(Keys.D9);

            _wasKeyPressed = new Dictionary<Keys, bool>();
            _wasKeyPressed.Add(Keys.D0, false);
            _wasKeyPressed.Add(Keys.D1, false);
            _wasKeyPressed.Add(Keys.D2, false);
            _wasKeyPressed.Add(Keys.D3, false);
            _wasKeyPressed.Add(Keys.D4, false);
            _wasKeyPressed.Add(Keys.D5, false);
            _wasKeyPressed.Add(Keys.D6, false);
            _wasKeyPressed.Add(Keys.D7, false);
            _wasKeyPressed.Add(Keys.D8, false);
            _wasKeyPressed.Add(Keys.D9, false);

            _numberOfPlayers = numPlayers;
            _currentPlayer = 0;

            _players = new List<Player>();
            _kingdomCards = new List<Card.Card>();

            for (int i = 0; i < _numberOfPlayers; i++)
            {
                _players.Add(new Player());
            }

            _masterSet = new List<Card.Card>();
            _masterSet.Add(new Card.Copper());
            _masterSet.Add(new Card.Silver());
            _masterSet.Add(new Card.Gold());
            _masterSet.Add(new Card.Estate());
            _masterSet.Add(new Card.Duchy());
            _masterSet.Add(new Card.Province());
            _masterSet.Add(new Card.Militia());
        }

        public void Reset()
        {
            _players.Clear();
            for (int i = 0; i < _numberOfPlayers; i++)
            {
                _players.Add(new Player());
            }
        }

        int LookupNumKey(Keys key)
        {
            switch (key)
            {
                case Keys.D0:
                    return 0;
                case Keys.D1:
                    return 1;
                case Keys.D2:
                    return 2;
                case Keys.D3:
                    return 3;
                case Keys.D4:
                    return 4;
                case Keys.D5:
                    return 5;
                case Keys.D6:
                    return 6;
                case Keys.D7:
                    return 7;
                case Keys.D8:
                    return 9;
                case Keys.D9:
                    return 9;
            }

            return -1;
        }

        public void UpdateInput()
        {
            foreach (Keys key in _numKeys)
            {
                bool up = Keyboard.GetState().IsKeyUp(key);
                if (up && _wasKeyPressed[key])
                {
                    _lastNumberPressed = LookupNumKey(key);
                    Console.WriteLine("[key pressed] = " + _lastNumberPressed);
                    break;
                }
            }

            foreach (Keys key in _numKeys)
            {
                bool down = Keyboard.GetState().IsKeyDown(key);
                _wasKeyPressed[key] = down;
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
