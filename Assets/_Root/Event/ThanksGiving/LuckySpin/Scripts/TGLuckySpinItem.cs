using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class TGLuckySpinItem : MonoBehaviour
{
    public TGLuckySpinType LuckySpinType;
    public int Value;
    public Vector2 ProbabilityRange;
    public Sprite Coin;
    public Sprite Turkey;
    public Sprite Skin;
    public Image Image;
    public TextMeshProUGUI Text;

    public void Setup()
    {
        if (LuckySpinType == TGLuckySpinType.Coin)
        {
            Image.sprite = Coin;
            Image.transform.localScale = Vector3.one * .7f;
            Text.text = Value.ToString();
        }
        else if (LuckySpinType == TGLuckySpinType.Turkey)
        {
            Image.sprite = Turkey;
            Image.transform.localScale = Vector3.one * .9f;
            Text.text = $"+{Value}";
        }
        else
        {
            Image.sprite = Skin;
            Image.transform.localScale = Vector3.one * .7f;
            Text.text = $"+{Value}";
        }
        Image.SetNativeSize();
    }
}

public enum TGLuckySpinType
{
    Coin,
    Skin,
    Turkey
}
