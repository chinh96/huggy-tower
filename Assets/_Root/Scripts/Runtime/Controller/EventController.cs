using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventController
{
    public static Action CoinTotalChanged;
    public static Action CurrentSkinHeroChanged;
    public static Action CurrentSkinPrincessChanged;
    public static Action<int> CastleBuilded;
    public static Action CastleReseted;
    public static Action SkinPopupReseted;
    public static Action CurrentLevelChanged;
}
