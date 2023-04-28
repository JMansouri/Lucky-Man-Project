using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace LuckyMan.Runtime
{
    internal class PlayerTurnHandler : ITurnHandler
    {
        private bool _isTurnComplete;
        public bool IsTurnComplete { get => _isTurnComplete; set => _isTurnComplete = value; }

        private int _currentDiceValue = 0;
        private int _diceValuesSum = 0;

        public TurnData HandleTurn()
        {
            // 1. read dice value:
            _currentDiceValue = DiceSimulator.GetRandomDiceNumber();

            // 2.add the dice value to the total dice values in this match:
            _diceValuesSum += _currentDiceValue;

            // 3. if sum >= 50 match is over othervise continue the match            
            if (_diceValuesSum >= 50) // win
            {
                _diceValuesSum = 50;
                return new TurnData(_currentDiceValue, 50); ;
            }
            else
            {
                _isTurnComplete = true;
                return new TurnData(_currentDiceValue, _diceValuesSum); ;
            }
        }
    }
}
