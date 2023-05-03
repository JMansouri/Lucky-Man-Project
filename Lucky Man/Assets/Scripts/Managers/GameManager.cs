using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LuckyMan.Runtime
{
    [RequireComponent(typeof(TurnManager))]
    public class GameManager : MonoBehaviour
    {
        private TurnManager _turnManager;
        [SerializeField] private UIManager _uiManager;

        private void Awake()
        {
            _turnManager = GetComponent<TurnManager>();
            _turnManager.InitializeTurns(Turn.Me);
        }

        private void OnEnable()
        {
            _uiManager.DiceButton.onClick.AddListener(PlayTurn);
        }

        private void OnDisable()
        {
            _uiManager.DiceButton.onClick.RemoveListener(PlayTurn);
        }

        public void PlayTurn()
        {
            // disable dice button
            _uiManager.EnableDiceButton(false);

            // TODO: show dice animation

            TurnData turnData = _turnManager.HandleCurrentTurn();
            Debug.Log(" dice : " + turnData.CurrentDice + " total : " + turnData.DiceSum);

            // update UI:
            if (_turnManager.CurrentTurn == Turn.Me)
            {
                _uiManager.SetMyLastDice(turnData.CurrentDice);
                _uiManager.SetMyTotalPoints(turnData.DiceSum);
            }
            else if (_turnManager.CurrentTurn == Turn.Opponent)
            {
                _uiManager.SetOpponentLastDice(turnData.CurrentDice);
                _uiManager.SetOpponentTotalPoints(turnData.DiceSum);
            }
        }

        public void ChangeTurn()
        {
            _turnManager.UpdateTurn();
            // enable dice button
            _uiManager.EnableDiceButton(true);
        }
    }
}
