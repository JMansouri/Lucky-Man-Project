namespace LuckyMan.Runtime
{
    public interface ITurnHandler
    {
        public bool IsTurnComplete { get; set; }

        public TurnData HandleTurn();
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