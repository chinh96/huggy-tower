using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardData : MonoBehaviour
{
    public static LeaderboardUserInfo UserInfoCurrent = new LeaderboardUserInfo();
    public static bool IsWorldTab = true;
    public static List<LeaderboardUserInfo> UserInfos = new List<LeaderboardUserInfo>();
}

public class LeaderboardUserInfo
{
    public Sprite Sprite;
    public string PlayerId;
    public string Name = "";
    public int Stat;
    public string CountryCode;
    public int Index;
}
