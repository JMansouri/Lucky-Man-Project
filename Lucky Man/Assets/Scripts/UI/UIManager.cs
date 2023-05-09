using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LuckyMan.Runtime
{
    public class UIManager : MonoBehaviour
    {
        //----------------------------------------------------------
        // UI elements
        //----------------------------------------------------------

        // ui panels
        [SerializeField] private BasePanel _startPanel;
        [SerializeField] private BasePanel _gameUIPanel;
        [SerializeField] private BasePanel _gameOverPanel;
        //public LeavePanel leavePanel;
        //public Text chatTextArea;

        // ui buttons
        [SerializeField] private Button _diceButton;
        public Button DiceButton => _diceButton;
        [SerializeField] private Button _startButton;
        public Button StartButton => _startButton;
        [SerializeField] private Button _returnToLobbyButton;
        public Button ReturnButton => _returnToLobbyButton;

        // ui changeble images
        [SerializeField] private Image _myPointsBar;
        [SerializeField] private Image _oppPointsBar;
        [SerializeField] private SpriteRenderer _diceGlow;

        // ui texts
        [SerializeField] private TextMeshProUGUI _myName;
        [SerializeField] private TextMeshProUGUI _oppName;
        [SerializeField] private TextMeshProUGUI _myLastDice;
        [SerializeField] private TextMeshProUGUI _oppLastDice;
        [SerializeField] private TextMeshProUGUI _myPoints;
        [SerializeField] private TextMeshProUGUI _oppPoints;
        [SerializeField] private TextMeshProUGUI _myNameVs;
        [SerializeField] private TextMeshProUGUI _oppNameVs;
        [SerializeField] private TextMeshProUGUI _gameOverText;
        [SerializeField] private TextMeshProUGUI _waitingForOppText;
        [SerializeField] private TextMeshProUGUI _startingIndicator;

        public void EnableDiceButton(bool status)
        {
            _diceButton.enabled = status;
            _diceGlow.gameObject.SetActive(status);
        }

        public void SetNames(string myName, string oppName)
        {
            _myNameVs.text = myName;
            _oppNameVs.text = oppName;
            _myName.text = myName;
            _oppName.text = oppName;
        }

        public void ShowWaitingForOpponent()
        {
            _startButton.gameObject.SetActive(false);
            _waitingForOppText.gameObject.SetActive(true);
        }

        public void HideStartPanel()
        {
            _startPanel.Hide();
        }

        public void ShowGameOverPanel(bool show, string gameOverText)
        {
            if (show)
            {
                _gameOverText.text = gameOverText;
                _gameOverPanel.Show();
            } else
            {
                _gameOverPanel.Hide();
            }
        }

        internal void UpdateMyUI(TurnData data)
        {
            _myLastDice.text = "Dice : " + data.CurrentDice;
            _myPoints.text = data.DiceSum.ToString();

            // animate the points bar fill
            _myPointsBar.fillAmount = (float)data.DiceSum / 50f;
        }

        internal void UpdateOppUI(TurnData data)
        {
            _oppLastDice.text = "Dice : " + data.CurrentDice;
            _oppPoints.text = data.DiceSum.ToString();

            // animate
            _oppPointsBar.fillAmount = (float)data.DiceSum / 50f;
        }

        internal void ShowGameUI(bool show)
        {
            if (show)
            {
                _gameUIPanel.Show();
            }
            else
            {
                _gameUIPanel.Hide();
            }
        }

        internal void ShowStartIndicator(bool myTurn)
        {
            //StartCoroutine(ShowIndicator());
        }

        IEnumerator ShowIndicator()
        {
            _startingIndicator.fontSize = 50f;
            _startingIndicator.gameObject.SetActive(true);
            while (_startingIndicator.fontSize <= 65f)
            {
                _startingIndicator.fontSize += 0.5f;
                yield return new WaitForSeconds(0.15f);
            }
            _startingIndicator.gameObject.SetActive(false);
            //EnableDiceButton(true);
        }
    }
}
