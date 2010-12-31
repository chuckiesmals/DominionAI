using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework.Input;

namespace DominionAI
{
    class Player
    {
        List<Card.Card> _deck;
        List<Card.Card> _discard;
        List<Card.Card> _hand;
        List<Keys> _numKeys;
        int _score = 0;

        public Phase _currentPhase = Phase.Idle;
        ActionState _actionState = ActionState.Idle;
        BuyState _buyState = BuyState.Idle;

        // Current Hand
        int _handTreasure = 0;
        int _treasureModifier = 0;
        int _buys = 0;
        int _actions = 0;
        // Card _currentActionCard;

        // Input handling
        Dictionary<Keys, bool> _wasKeyPressed;

        public enum Phase
        {
            Idle,
            Action,
            Buy,
            Cleanup
        }

        enum ActionState
        {
            Idle,
            Choose,
            Perform
        }

        enum BuyState
        {
            Idle,
            Choose
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

        int CheckNumberInput()
        {
            int keyPressed = -1;

            foreach (Keys key in _numKeys)
            {
                bool up = Keyboard.GetState().IsKeyUp(key);
                if (up && _wasKeyPressed[key])
                {
                    keyPressed = LookupNumKey(key);
                    break;
                }
            }

            foreach (Keys key in _numKeys)
            {
                bool down = Keyboard.GetState().IsKeyDown(key);
                _wasKeyPressed[key] = down;
            }

            return keyPressed;
        }

        public Player()
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

            _deck = new List<Card.Card>();
            _discard = new List<Card.Card>();
            _hand = new List<Card.Card>();

            for (int i = 0; i < 7; i++)
            {
                _deck.Add(new Card.Copper());
            }

            for (int i = 0; i < 3; i++)
            {
                _deck.Add(new Card.Estate());
            }

            ShuffleCards(_deck);
        }

        void CheckToReshuffle()
        {
            if (_deck.Count == 0)
            {
                foreach (Card.Card card in _discard)
                {
                    _deck.Add(card);
                }
                _discard.Clear();

                // TODO: shuffle deck
                ShuffleCards(_deck);
            }
        }

        void ShuffleCards(List<Card.Card> cards)
        {
            //Console.WriteLine("Cards Before:");

            //foreach (Card.Card card in cards)
            //{
            //    Console.WriteLine("  " + card._name);
            //}

            Random random = new Random();

            for (int i = 0; i < cards.Count; ++i)
            {
                Card.Card card = cards[i];
                int randomIndex = random.Next(cards.Count - 1);
                cards[i] = cards[randomIndex];
                cards[randomIndex] = card;
            }

            //Console.WriteLine("Cards After:");

            //foreach (Card.Card card in cards)
            //{
            //    Console.WriteLine("  " + card._name);
            //}
        }

        void DrawHand()
        {
            Debug.Assert(_hand.Count == 0, "Hand is not empty.");

            for (int i = 0; i < 5; i++)
            {
                CheckToReshuffle();
                Card.Card card = _deck[0];
                _hand.Add(card);
                _deck.RemoveAt(0);
            }

            Console.WriteLine("    Draw Hand:");
            foreach(Card.Card card in _hand)
            {
                Console.WriteLine("      " + card._name);
            }
        }

        public void Update(Simulator sim)
        {
            Console.WriteLine("  " + _currentPhase.ToString());

            if (_currentPhase == Phase.Idle)
            {
                // If hand is empty, pick first five cards
                DrawHand();
                _currentPhase = Phase.Action;
            }
            // Action Phase
            else if (_currentPhase == Phase.Action)
            {
                UpdateActionState(sim);
            }
            // Buy Phase
            else if (_currentPhase == Phase.Buy)
            {
                UpdateBuyState(sim);
            }
            // Put hand into Discard pile and Fill hand
            else if (_currentPhase == Phase.Cleanup)
            {
                foreach (Card.Card card in _hand)
                {
                    _discard.Add(card);
                }
                _hand.Clear();

                _currentPhase = Phase.Idle;
                sim.NextPlayerTurn();
            }
        }

        bool BuyCard(int index, Simulator sim)
        {
            bool bought = false;
            Card.Card cardToBuy = sim._masterSet[index];

            if (sim._masterSet[index]._supplyCount > 0 && _handTreasure >= cardToBuy._cost)
            {
                Console.WriteLine("    Buying " + cardToBuy._name);
                _discard.Add(cardToBuy);
                cardToBuy._supplyCount--;

                _handTreasure = -cardToBuy._cost;
                bought = true;
            }
            else
            {
                Console.WriteLine("    Can't buy: " + sim._masterSet[index]._name);
            }

            return bought;
        }

        void UpdateBuyState(Simulator sim)
        {
            Console.WriteLine("    " + _buyState.ToString());
            if (_buyState == BuyState.Idle)
            {
                // Print cards available to buy
                _handTreasure = CountMoneyFromTreasure();
                Console.WriteLine("      Treasure: " + _handTreasure);
                PrintCardsToBuy(sim);
                _buyState = BuyState.Choose;                
            }
            else if (_buyState == BuyState.Choose)
            {
                // Wait for input
                if (sim._lastNumberPressed > -1)
                {
                    Console.WriteLine("      Pressed: " + sim._lastNumberPressed);

                    if (BuyCard(sim._lastNumberPressed, sim))
                    {
                        _buyState = BuyState.Idle;
                        _currentPhase = Phase.Cleanup;
                    }

                    sim._lastNumberPressed = -1;
                }
            }

        }

        void PrintCardsToBuy(Simulator sim)
        {
            for (int i = 0; i < sim._masterSet.Count; i++)
            {
                Card.Card card = sim._masterSet[i];
                Console.Write("      [" + i + "] [" + card._supplyCount + "] " + card._name);
                if (card._cost <= _handTreasure)
                {
                    Console.Write(" [BUY?]");
                }

                Console.Write("\n");
            }
        }

        int CountMoneyFromTreasure()
        {
            int treasure = 0;

            foreach (Card.Card card in _hand)
            {
                treasure += card._worth;    
            }

            return treasure;
        }

        public void UpdateActionState(Simulator sim)
        {
            Console.WriteLine("    " + _actionState.ToString());

            if (_actionState == ActionState.Idle)
            {
                // 1. List Actions in hand                
                _actionState = ActionState.Choose;
            }
            else if (_actionState == ActionState.Choose)
            {
                // 2. Choose Action
                _actionState = ActionState.Perform;
                
            }
            else if (_actionState == ActionState.Perform)
            {
                // 3. Perform Action
                // 4. More actions? Repeat #2
                _actionState = ActionState.Idle;          
                _currentPhase = Phase.Buy;
            }
        }
    }
}
