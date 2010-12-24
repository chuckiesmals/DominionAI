using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DominionAI
{
    class Player
    {
        List<Card> _deck;
        List<Card> _discard;
        List<Card> _hand;
        int _score;
        public Phase _currentPhase = Phase.Idle;
        ActionState _actionState = ActionState.Idle;

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

        public void Update(Simulator sim)
        {
            Console.WriteLine("  " + _currentPhase.ToString());

            if (_currentPhase == Phase.Idle)
            {
                // If hand is empty, pick first five cards
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
                // 1. Have money?
                // 2. Buy card            
                
                _currentPhase = Phase.Cleanup;    
            }
            // Put hand into Discard pile and Fill hand
            else if (_currentPhase == Phase.Cleanup)
            {
                _currentPhase = Phase.Idle;
                sim.NextPlayerTurn();
            }
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
