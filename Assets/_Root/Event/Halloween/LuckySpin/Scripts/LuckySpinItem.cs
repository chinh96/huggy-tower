using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuckySpinItem : MonoBehaviour
{
    public LuckySpinType LuckySpinType;
    public int Value;
    public Vector2 ProbabilityRange;
}

public enum LuckySpinType
{
    Coin,
    Candy
}
