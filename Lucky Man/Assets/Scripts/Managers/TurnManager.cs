using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LuckyMan.Runtime
{
    public enum Turn
    {
        Me,
        Opponent,
        Neither
    }

    public class TurnManager : MonoBehaviour
    {
        private Turn _currentTurn = Turn.Neither;
        public Turn CurrentTurn => _currentTurn;

        private ITurnHandler _myTurnHandler = new PlayerTurnHandler();
        private ITurnHandler _opponentTurnHandler = new PlayerTurnHandler();

        public void InitializeTurns(Turn startingTurn)
        {
            _currentTurn = startingTurn;
        }

        public TurnData HandleCurrentTurn()
        {
            if (_currentTurn == Turn.Me && _myTurnHandler != null)
            {
                Debug.Log("My Turn");
                return PlayMyTurn();
            }
            else if (_currentTurn == Turn.Opponent && _opponentTurnHandler != null)
            {
                return PlayOpponentTurn();
            }
            return new TurnData();
        }

        private TurnData PlayMyTurn()
        {
            _myTurnHandler.IsTurnComplete = false;
            return _myTurnHandler.HandleTurn();
        }

        private TurnData PlayOpponentTurn()
        {
            _opponentTurnHandler.IsTurnComplete = false;
            return _opponentTurnHandler.HandleTurn();
        }

        public void UpdateTurn()
        {
            if (_currentTurn == Turn.Me && _myTurnHandler.IsTurnComplete)
            {
                _currentTurn = Turn.Opponent;
            }
            else if (_currentTurn == Turn.Opponent && _opponentTurnHandler.IsTurnComplete)
            {
                _currentTurn = Turn.Me;
            }
        }
    }
}