using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DominionAI
{
    namespace Card
    {
        public enum Type
        {
            Action,
            Treasure,
            Victory
        }

        public class Card
        {
            public int _cost = 0;
            public int _worth = 0;
            public int _vp = 0;
            public int _supplyCount = 10;
            public string _name;
            public Type _type;
        }

        #region Treasure
        public class Copper : Card
        {
            public Copper()
            {
                _worth = 1;
                _name = "Copper";
                _type = Type.Treasure;
            }
        }

        public class Silver : Card
        {
            public Silver()
            {
                _cost = 3;
                _worth = 2;
                _name = "Silver";
                _type = Type.Treasure;
            }
        }

        public class Gold : Card
        {
            public Gold()
            {
                _cost = 6;
                _worth = 3;
                _name = "Gold";
                _type = Type.Treasure;
            }
        }
        #endregion

        #region Victory Points
        public class Estate : Card
        {
            public Estate()
            {
                _cost = 2;
                _worth = 0;
                _vp = 1;
                _name = "Estate";
                _type = Type.Victory;
            }
        }

        public class Duchy : Card
        {
            public Duchy()
            {
                _cost = 5;
                _worth = 0;
                _vp = 3;
                _name = "Duchy";
                _type = Type.Victory;
            }
        }

        public class Province : Card
        {
            public Province()
            {
                _cost = 8;
                _worth = 0;
                _vp = 6;
                _name = "Province";
                _type = Type.Victory;
            }
        }
        #endregion
     
        public abstract class Action : Card
        {
            abstract public void PerformAction(Player player, Simulator sim);

            abstract public bool Update(Player player, Simulator sim);
        }

        #region Base Set Actions
        public class Militia : Action
        {
            MilitiaPhase _currentPhase = MilitiaPhase.Idle;
            int _currentPlayerToDiscard = -1;

            public Militia()
            {
                _cost = 4;
                _worth = 0;
                _vp = 0;
                _name = "Militia";
                _type = Type.Action;
            }

            enum MilitiaPhase 
            {
                Idle,
                PrintCards,
                DiscardTwoCards
            }

            override public void PerformAction(Player owner, Simulator sim)
            {
                Console.Write(" begins " + _name + " action.\n");

                // +2 Money
                owner._treasureModifier += 2;

                // Invoke Militia phase
                _currentPhase = MilitiaPhase.DiscardTwoCards;
                _currentPlayerToDiscard = 0;

                if (sim._players[_currentPlayerToDiscard] == owner)
                {
                    _currentPlayerToDiscard++;
                }
            }

            override public bool Update(Player owner, Simulator sim)
            {
                bool done = false;

                Player playerDiscarding = sim._players[_currentPlayerToDiscard];
                if (_currentPhase == MilitiaPhase.DiscardTwoCards)
                {
                    if (playerDiscarding._hand.Count > 3)
                    {
                        Console.WriteLine("      Player " + _currentPlayerToDiscard + " needs to discard 2 cards");
                        playerDiscarding.PrintHand(8);

                        if (sim._lastNumberPressed > -1)
                        {
                            Console.WriteLine("  Selected: " + sim._lastNumberPressed);
                            
                            // Discard card from hand
                            Card card = playerDiscarding._hand[sim._lastNumberPressed];
                            playerDiscarding._discard.Add(card);
                            playerDiscarding._hand.RemoveAt(sim._lastNumberPressed);

                            sim._lastNumberPressed = -1;
                        }
                    }
                    else
                    {
                        // That player has discarded, move to next player
                        _currentPlayerToDiscard++;

                        if (sim._players.Count > _currentPlayerToDiscard)
                        {
                            // skip ahead by 2, if this is the owner
                            if (sim._players[_currentPlayerToDiscard] == owner)
                            {
                                _currentPlayerToDiscard += 2;
                            }
                            // skip ahead once
                            else
                            {
                                _currentPlayerToDiscard++;
                            }
                        }
                        else
                        {
                            _currentPlayerToDiscard = -1;
                            _currentPhase = MilitiaPhase.Idle;
                            done = true;
                        }

                        // check bounds
                        if (_currentPlayerToDiscard >= sim._numberOfPlayers)
                        {
                            _currentPlayerToDiscard = -1;
                            done = true;
                        }
                    }
                }

                return done;
            }
        }
        #endregion   
    }

}
