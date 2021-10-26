using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class LuckySpinItem : MonoBehaviour
{
    public LuckySpinType LuckySpinType;
    public int Value;
    public Vector2 ProbabilityRange;
    public Sprite Coin;
    public Sprite Candy;
    public Image Image;
    public TextMeshProUGUI Text;

    public void Setup()
    {
        if (LuckySpinType == LuckySpinType.Coin)
        {
            Image.sprite = Coin;
            Text.text = Value.ToString();
        }
        else
        {
            Image.sprite = Candy;
            Text.text = $"+{Value}";
        }
        Image.SetNativeSize();
    }

    public void Receive(Action action)
    {
        if (LuckySpinType == LuckySpinType.Coin)
        {
            Data.CoinTotal += Value;
        }
        else
        {
            Data.TotalGoldMedal += Value;
        }
    }
}

public enum LuckySpinType
{
    Coin,
    Candy
}
