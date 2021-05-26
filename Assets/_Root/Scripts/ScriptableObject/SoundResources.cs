using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "SoundResources", menuName = "ScriptableObjects/SoundResources")]
public class SoundResources : ScriptableObject
{
    private static SoundResources instance;
    public static SoundResources Instance => instance ? instance : instance = Resources.Load<SoundResources>("SoundResources");

    public List<SoundData> SoundDatas;
}

[Serializable]
public class SoundData
{
    public SoundType SoundType;
    public AudioClip Clip;
}

public enum SoundType
{
    BackgroundInGame,
    BackgroundHome,
    BackgroundCastle,
    ButtonClick,
    Win,
    Lose,
    OpenPopup,
    HeroDrag,
    HeroDrop,
    HeroHit,
    HeroCut,
    HeroUpLevel,
    HeroDie,
    HeroYeah,
    EnemyHit,
    EnemyCut,
    EnemyShoot,
    EnemyBite,
    OpenChest,
    RescuePrincess,
    BlockWallBreak,
    CoinMove,
    BuildItem,
    HeroDownLevel
}
