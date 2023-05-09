using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class PlayerListItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _rankText;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _pointsText;

    public void Init(string rank, string name, string points)
    {
        _rankText.text = rank;
        _nameText.text = name;
        _pointsText.text = points + " امتیاز";
    }

    public void Clear()
    {
        Destroy(this);
    }
}

