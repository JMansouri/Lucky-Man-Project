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
        //public LeavePanel leavePanel;
        //public Text chatTextArea;

        // ui buttons
        [SerializeField] private Button _diceButton;
        public Button DiceButton => _diceButton;
        [SerializeField] private Button _startButton;
        public Button StartButton => _startButton;

        // ui changeble images
        [SerializeField] private Image _myPointsBar;
        [SerializeField] private Image _oppPointsBar;

        // ui texts
        [SerializeField] private TextMeshProUGUI _myName;
        [SerializeField] private TextMeshProUGUI _oppName;
        [SerializeField] private TextMeshProUGUI _myLastDice;
        [SerializeField] private TextMeshProUGUI _oppLastDice;
        [SerializeField] private TextMeshProUGUI _myPoints;
        [SerializeField] private TextMeshProUGUI _oppPoints;
        [SerializeField] private TextMeshProUGUI _myNameVs;
        [SerializeField] private TextMeshProUGUI _oppNameVs;

        public void EnableDiceButton(bool status)
        {
            _diceButton.enabled = status;
        }

        public void SetNames(string myName, string oppName)
        {
            _myNameVs.text = myName;
            _oppNameVs.text = oppName;
            _myName.text = myName;
            _oppName.text = oppName;
        }

        public void HideStartPanel()
        {
            _startPanel.Hide();
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
    }
}
