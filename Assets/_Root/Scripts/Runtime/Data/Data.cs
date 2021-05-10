using UnityEngine;

namespace Lance.TowerWar.Data
{
    public static class Data
    {
        #region playerprefs

        private const string CURRENT_LEVEL_KEY = "current_level";

        public static int UserCurrentLevel { get => PlayerPrefs.GetInt(CURRENT_LEVEL_KEY, 0); set => PlayerPrefs.SetInt(CURRENT_LEVEL_KEY, value); }

        private const string MAX_LEVEL_KEY = "max_level";
        public static int UserMaxLevel { get => PlayerPrefs.GetInt(MAX_LEVEL_KEY, 0); set => PlayerPrefs.SetInt(MAX_LEVEL_KEY, value); }
        
        private const string CURRENT_SKIN_HERO = "current_skin_hero";
        public static int CurrentSkinHero { get => PlayerPrefs.GetInt(CURRENT_SKIN_HERO, 0); set => PlayerPrefs.SetInt(CURRENT_SKIN_HERO, value); }

        private const string SOUND_STATE_KEY = "sound_state";

        /// <summary>
        /// 
        /// </summary>
        public static bool UserSound { get => GetBool(SOUND_STATE_KEY, true); set => SetBool(SOUND_STATE_KEY, value); }


        private const string MUSIC_STATE_KEY = "music_state";

        /// <summary>
        /// 
        /// </summary>
        public static bool UserMusic { get => GetBool(MUSIC_STATE_KEY, true); set => SetBool(MUSIC_STATE_KEY, value); }


        private const string VIBRATE_STATE_KEY = "vibrate_state";

        /// <summary>
        /// 
        /// </summary>
        public static bool UserVibrate { get => GetBool(VIBRATE_STATE_KEY, false); set => SetBool(VIBRATE_STATE_KEY, value); }


        private const string ONLY_USE_ADMOB = "only_use_admob";

        /// <summary>
        /// 
        /// </summary>
        public static bool OnlyUseAdmob { get => GetBool(ONLY_USE_ADMOB, false); set => SetBool(ONLY_USE_ADMOB, value); }


        private const string REMOVE_ADS_STATE = "remove_ads";

        /// <summary>
        /// 
        /// </summary>
        public static bool RemoveAds { get => GetBool(REMOVE_ADS_STATE, false); set => SetBool(REMOVE_ADS_STATE, value); }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private static bool GetBool(string key, bool defaultValue = false)
        {
            var value = PlayerPrefs.GetInt(key, defaultValue ? 1 : 0);
            return value > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        public static void SetBool(string id, bool value) { PlayerPrefs.SetInt(id, value ? 1 : 0); }


        public const string CACHE_LEVEL_INDEX = "cache_level_index";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static int GetCacheLevelIndex(int index) { return PlayerPrefs.GetInt($"{CACHE_LEVEL_INDEX}_{index}", 0); }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="level"></param>
        public static void SetCacheLevelIndex(int index, int level) { PlayerPrefs.SetInt($"{CACHE_LEVEL_INDEX}_{index}", level); }


        private const string COUNT_PLAY_LEVEL = "count_play_level";
        public static int CountPlayLevel { get => PlayerPrefs.GetInt(COUNT_PLAY_LEVEL, 0); set => PlayerPrefs.SetInt(COUNT_PLAY_LEVEL, value); }

        #endregion
    }
}