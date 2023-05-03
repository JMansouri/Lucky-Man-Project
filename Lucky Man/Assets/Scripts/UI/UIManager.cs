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

        public void SetMyLastDice(int diceNumber)
        {
            _myLastDice.text = "Dice : " + diceNumber.ToString();
        }

        public void SetOpponentLastDice(int diceNumber)
        {
            _oppLastDice.text = "Dice : " + diceNumber.ToString();
        }

        public void SetMyTotalPoints(int totalPoints)
        {
            _myPoints.text = totalPoints.ToString();
            _myPointsBar.fillAmount = (float)totalPoints / 50f;
        }

        public void SetOpponentTotalPoints(int totalPoints)
        {
            _oppPoints.text = totalPoints.ToString();
            _oppPointsBar.fillAmount = (float)totalPoints / 50f;
        }

        public void EnableDiceButton(bool status)
        {
            _diceButton.enabled = status;
        }

        public void HideStartPanel()
        {
            _startPanel.Hide();
        }
    }
}
