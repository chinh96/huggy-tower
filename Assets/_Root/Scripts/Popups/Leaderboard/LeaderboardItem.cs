using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LeaderboardItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI index;
    [SerializeField] private TextMeshProUGUI name;
    [SerializeField] private Image flag;
    [SerializeField] private TextMeshProUGUI level;

    public void Init(LeaderboardUserInfo userInfo)
    {
        index.text = userInfo.Position.ToString();
        name.text = userInfo.Name;
        flag.sprite = userInfo.Sprite;
        level.text = userInfo.Stat.ToString();
    }
}
