namespace Lance.TowerWar.Data
{
    using UnityEngine;
    
    public class HeroSkinData : ScriptableObject
    {
        private static HeroSkinData instance;
        public static HeroSkinData Instance => instance ? instance : instance = Resources.Load<HeroSkinData>("HeroSkinData");

        public static string SkinHeroByIndex(int index) { return $"Hero{index + 1}"; }
    }
}