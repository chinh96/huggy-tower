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
    [SerializeField] private GameObject backgroundNormal;
    [SerializeField] private GameObject backgroundCurrent;
    [SerializeField] private GameObject rank1;
    [SerializeField] private GameObject rank2;
    [SerializeField] private GameObject rank3;

    public void Init(LeaderboardUserInfo userInfo)
    {
        index.text = userInfo.Index.ToString();
        name.text = userInfo.Name;
        flag.sprite = userInfo.Sprite;
        level.text = userInfo.Stat.ToString();

        backgroundCurrent.SetActive(userInfo.PlayerId == Data.PlayerId);
        rank1.SetActive(userInfo.Index == 1);
        rank2.SetActive(userInfo.Index == 2);
        rank3.SetActive(userInfo.Index == 3);
    }
}
