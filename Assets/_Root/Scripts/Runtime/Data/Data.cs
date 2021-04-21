using UnityEngine;

namespace Lance.TowerWar.Data
{
    public static class Data
    {
        #region playerprefs

        private const string CURRENT_LEVEL_KEY = "current_level";

        public static int UserCurrentLevel { get => PlayerPrefs.GetInt(CURRENT_LEVEL_KEY, 0); set => PlayerPrefs.SetInt(CURRENT_LEVEL_KEY, value); }


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

        #endregion
    }
}