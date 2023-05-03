using LuckyMan.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LuckyMan.Runtime
{
    public class GameManager
    {
        public int MyPlayerId { get; set; }
        public int OppPlayerId { get; set; }

        private PlayerState _myState;
        private PlayerState _oppState;

        private Turn _currentTurn = Turn.Neither;
        public Turn CurrentTurn => _currentTurn;


        public void InitializeGame(int startingPlayerId)
        {
            _myState = new PlayerState();
            _oppState = new PlayerState();
            _myState.ResetState();
            _oppState.ResetState();

            UpdateTurn(startingPlayerId);
        }

        public TurnData UpdateState(int dice)
        {
            if (IsMyTurn())
            {
                return _myState.UpdateState(dice);
            }
            else if (_currentTurn == Turn.Opponent)
            {
                return _oppState.UpdateState(dice);
            }
            else
            {
                Debug.Log("There is a problem with turns in update state!");
                return new TurnData();
            }
        }

        public void UpdateTurn(int playerId)
        {
            if (playerId == MyPlayerId)
            {
                _currentTurn = Turn.Me;
            }
            else if (playerId == OppPlayerId)
            {
                _currentTurn = Turn.Opponent;
            }
            else
            {
                Debug.Log("There is a problem in update turn, player id wont match!");
            }
        }

        public bool IsMyTurn()
        {
            return _currentTurn == Turn.Me;
        }
    }

    public enum Turn
    {
        Me,
        Opponent,
        Neither
    }
}