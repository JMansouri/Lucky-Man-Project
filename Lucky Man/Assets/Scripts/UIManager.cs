using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LuckyMan.Runtime
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private Button _diceButton;
        [SerializeField] private Button _passTurnButton;
        [SerializeField] private Image _myPointsBar;
        [SerializeField] private Image _oppPointsBar;
        [SerializeField] private TextMeshProUGUI _myName;
        [SerializeField] private TextMeshProUGUI _oppName;
        [SerializeField] private TextMeshProUGUI _myCurrentDiceValue;
        [SerializeField] private TextMeshProUGUI _oppCurrentDiceValue;
        [SerializeField] private TextMeshProUGUI _myTotalDiceValues;
        [SerializeField] private TextMeshProUGUI _oppTotalDiceValues;

        public Button DiceButton => _diceButton;
        public Button PassTurnButton => _passTurnButton;

        public void SetMyCurrentDiceValue(int diceNumber)
        {
            _myCurrentDiceValue.text = "Dice : " + diceNumber.ToString();
        }

        public void SetOpponentCurrentDiceValue(int diceNumber)
        {
            _oppCurrentDiceValue.text = "Dice : " + diceNumber.ToString();
        }

        public void SetMyTotalPoints(int totalPoints)
        {
            _myTotalDiceValues.text = totalPoints.ToString();
            _myPointsBar.fillAmount = (float)totalPoints / 50f;
        }

        public void SetOpponentTotalPoints(int totalPoints)
        {
            _oppTotalDiceValues.text = totalPoints.ToString();
            _oppPointsBar.fillAmount = (float)totalPoints / 50f;
        }

        public void EnableDiceButton(bool status)
        {
            _diceButton.enabled = status;
        }

        public void EnablePassTurnButton(bool status)
        {
            _passTurnButton.enabled = status;
        }
    }
}
