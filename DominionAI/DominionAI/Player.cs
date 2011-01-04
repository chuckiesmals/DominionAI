using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework.Input;

namespace DominionAI
{
    public class Player
    {
        public List<Card.Card> _deck;
        public List<Card.Card> _discard;
        public List<Card.Card> _hand;
        List<Keys> _numKeys;
        int _score = 0;

        public Phase _currentPhase = Phase.Idle;
        ActionState _actionState = ActionState.Idle;
        BuyState _buyState = BuyState.Idle;

        // Current Hand
        int _handTreasure = 0;
        public int _treasureModifier = 0;
        int _numberOfBuys = 0;
        int _numberOfActions = 0;
        Card.Action _currentAction;

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

            _deck.Add(new Card.Militia());

            ShuffleCards(_deck);

            DrawHand();
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
            Console.WriteLine("  Shuffling cards");
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

            //Console.WriteLine("\n  Draw Hand:");
            //foreach(Card.Card card in _hand)
            //{
            //    Console.WriteLine("      " + card._name);
            //}
        }

        void PrintNakedHand(int indent)
        {
            string spacing = "";
            for (int i = 0; i < indent; ++i)
            {
                spacing += " ";
            }

            for (int i = 0; i < _hand.Count; ++i)
            {
                Console.Write(spacing + _hand[i]._name + " ");
            }

            Console.Write("\n");
        }

        public void Update(Simulator sim)
        {
            //Console.WriteLine("  " + _currentPhase.ToString());

            if (_currentPhase == Phase.Idle)
            {
                Console.Write(" turn starts.\n");
                // If hand is empty, pick first five cards
                if (_hand.Count == 0)
                {
                    DrawHand();
                }

                PrintNakedHand(2);

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
                Console.Write(" cleans up.\n");

                foreach (Card.Card card in _hand)
                {
                    _discard.Add(card);
                }

                _hand.Clear();
                _handTreasure = 0;
                _treasureModifier = 0;
                DrawHand();
                _currentPhase = Phase.Idle;
                sim.NextPlayerTurn();
            }
        }

        bool BuyCard(int index, Simulator sim)
        {
            bool bought = false;
            Card.Card cardToBuy = sim._masterSet[index];

            if (sim._masterSet[index]._supplyCount > 0 && _handTreasure + _treasureModifier >= cardToBuy._cost)
            {
                Console.WriteLine(" bought " + cardToBuy._name + ".");
                _discard.Add(cardToBuy);
                cardToBuy._supplyCount--;

                _handTreasure = -cardToBuy._cost;
                bought = true;
            }
            else
            {
                Console.WriteLine(" can't buy " + sim._masterSet[index]._name + ".\n");
            }

            return bought;
        }

        void UpdateBuyState(Simulator sim)
        {
            //Console.WriteLine("    " + _buyState.ToString());

            if (_buyState == BuyState.Idle)
            {
                Console.WriteLine(" buy a card:");
                // Print cards available to buy
                _handTreasure = CountMoneyFromTreasure();
                Console.WriteLine("      Treasure: " + _handTreasure);
                Console.WriteLine("      Treasure Modifier: " + _treasureModifier);
                PrintCardsToBuy(sim);
                _buyState = BuyState.Choose;                
            }
            else if (_buyState == BuyState.Choose)
            {
                // Wait for input
                if (sim._lastNumberPressed > -1)
                {
                    //Console.WriteLine("      Pressed: " + sim._lastNumberPressed);

                    if (BuyCard(sim._lastNumberPressed, sim))
                    {
                        _buyState = BuyState.Idle;
                        _currentPhase = Phase.Cleanup;
                    }

                    sim._lastNumberPressed = -1;
                }
                else
                {
                    Console.Write(" buy a card.\n");
                }
            }

        }

        void PrintCardsToBuy(Simulator sim)
        {
            for (int i = 0; i < sim._masterSet.Count; i++)
            {
                Card.Card card = sim._masterSet[i];
                Console.Write("      [" + i + "] [" + card._supplyCount + "] " + card._name);
                if (card._cost <= _handTreasure + _treasureModifier)
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

        bool AreThereActions()
        {
            for (int i = 0; i < _hand.Count; ++i)
            {
                if (_hand[i]._type == Card.Type.Action)
                {
                    return true;
                }
            }

            return false;
        }

        public void UpdateActionState(Simulator sim)
        {
            //Console.WriteLine("    " + _actionState.ToString());

            if (_actionState == ActionState.Idle)
            {
                if (AreThereActions())
                {
                    // 1. List Actions in hand
                    _actionState = ActionState.Choose;
                    PrintActions(6);
                }
                else
                {
                    // go to buy phase if no actions
                    Console.Write(" no actions, go to buy phase.\n");
                    _currentPhase = Phase.Buy;
                }
            }
            else if (_actionState == ActionState.Choose)
            {
                // Wait for input
                if (sim._lastNumberPressed > -1)
                {
                    // 2. Choose Action
                    _actionState = ActionState.Perform;

                    //Console.WriteLine("      Pressed: " + sim._lastNumberPressed);
                    ChooseAction(sim._lastNumberPressed, sim);
                }
                else
                {
                    Console.Write(" needs to play an action or skip.\n");
                }
            }
            else if (_actionState == ActionState.Perform)
            {
                if (_currentAction != null)
                {
                    Console.WriteLine(" is playing " + _currentAction._name + ".");

                    bool done = _currentAction.Update(this, sim);

                    // TODO: More actions? Repeat #2

                    if (done)
                    {
                        _currentAction = null;
                        _actionState = ActionState.Idle;
                        _currentPhase = Phase.Buy;
                    }
                }
                else
                {
                    _actionState = ActionState.Idle;
                    _currentPhase = Phase.Buy;
                }
            }
        }

        bool ChooseAction(int index, Simulator sim)
        {
            bool success = false;

            if (index < _hand.Count && _hand[index]._type == Card.Type.Action)
            {
                Card.Action action = (Card.Action)_hand[index];
                action.PerformAction(this, sim);
                _currentAction = action;
                sim._lastNumberPressed = -1;
                success = true;
            }

            return success;
        }

        public void PrintActions(int indent)
        {
            Console.Write(" needs to choose an action: \n");
            string spaces = "";
            for (int i = 0; i < indent; ++i)
            {
                spaces += " ";
            }

            for (int i = 0; i < _hand.Count; ++i)
            {
                if (_hand[i]._type == Card.Type.Action)
                {
                    Console.WriteLine(spaces + "[" + i + "]" + " " + _hand[i]._name);
                }
            }
        }

        public void PrintHand(int indent)
        {
            string spaces = "";
            for (int i = 0; i < indent; i++)
            {
                spaces += " ";
            }

            for (int i = 0; i < _hand.Count; ++i)
            {
                Console.WriteLine(spaces + "[" + i + "] " + _hand[i]._name);
            }
        }
    }
}
