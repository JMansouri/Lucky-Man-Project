namespace LuckyMan.Runtime
{
    internal class PlayerState
    {
        private int _lastDice = 0;
        private int _totalPoints = 0;

        public void ResetState()
        {
            _lastDice = 0;
            _totalPoints = 0;
        }

        public TurnData UpdateState(int dice)
        {
            // 1. read dice value:
            _lastDice = dice;

            // 2.add the dice value to the total dice values in this match:
            _totalPoints += dice;

            // 3. if sum >= 50 match is over othervise continue the match            
            if (_totalPoints >= 50) // win
            {
                _totalPoints = 50;
                return new TurnData(_lastDice, 50); ;
            }
            else
            {
                return new TurnData(_lastDice, _totalPoints); ;
            }
        }
    }

    public struct TurnData
    {
        private int _currentDice;
        private int _diceSum;

        public int CurrentDice => _currentDice;
        public int DiceSum => _diceSum;

        public TurnData(int currentDice, int diceSum)
        {
            _currentDice = currentDice;
            _diceSum = diceSum;
        }
    }
}
