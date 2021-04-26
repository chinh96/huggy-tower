using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Lance.Common;
using Lance.TowerWar.LevelBase;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Lance.TowerWar.Data
{
    public class DataBridge : Singleton<DataBridge>
    {
        #region properties

        [SerializeField, ReadOnly] private LevelMap previousLevelLoaded = null;
        [SerializeField, ReadOnly] private LevelMap nextLevelLoaded = null;

        public LevelMap PreviousLevelLoaded { get => previousLevelLoaded; set => previousLevelLoaded = value; }
        public LevelMap NextLevelLoaded { get => nextLevelLoaded; set => nextLevelLoaded = value; }
        private int[] _cacheLevels;

        #endregion

        #region unity-api

        private void Start() { CheckCacheLevel(); }

        #endregion

        #region function

        /// <summary>
        /// check cache level
        /// </summary>
        public void CheckCacheLevel()
        {
            bool flagOutLevel = false;
            if (Data.UserCurrentLevel > Config.MaxLevelCanReach)
            {
                _cacheLevels = new int[Config.MaxLevelWithOutTutorial];

                for (int i = 0; i < Config.MaxLevelWithOutTutorial; i++)
                {
                    _cacheLevels[i] = Data.GetCacheLevelIndex(i);
                    if (_cacheLevels[i] >= Config.MaxLevelCanReach) flagOutLevel = true;
                }

                if (flagOutLevel) MakeCacheLevel();
            }
        }

        /// <summary>
        /// make cache level
        /// </summary>
        public void MakeCacheLevel()
        {
            var tempList = new List<int>();
            for (int i = 0; i < Config.MaxLevelCanReach; i++)
            {
                if (Config.LevelSkips.Contains(i)) continue;
                tempList.Add(i);
            }

            tempList.Shuffle();

            _cacheLevels = new int[Config.MaxLevelWithOutTutorial];

            for (int i = 0; i < tempList.Count; i++)
            {
                Data.SetCacheLevelIndex(i, tempList[i]);
                _cacheLevels[i] = tempList[i];
            }
        }

        /// <summary>
        /// return level prefab and real level index
        /// </summary>
        /// <param name="levelIndex"></param>
        /// <returns></returns>
        public async UniTask<(GameObject, int)> GetLevel(int levelIndex)
        {
            if (Data.OnlyUseAdmob)
            {
                //if (AdsManager.Instance != null) AdsManager.Instance.RequestReward();
            }
#if IRONSOURCE
        else
        {
            //if (AdsIronSourceManager.Instance != null) AdsIronSourceManager.Instance.RequestReward();
        }
#endif

            if (levelIndex > Config.MaxLevelCanReach - 1)
            {
                var temp = (levelIndex - Config.MaxLevelCanReach) % (Config.MaxLevelWithOutTutorial - 1);

                if (Data.CountPlayLevel >= Config.MaxLevelWithOutTutorial)
                {
                    MakeCacheLevel();
                    Data.CountPlayLevel = 0;
                }
                else
                {
                    if (_cacheLevels == null || _cacheLevels.Length == 0 || _cacheLevels.Count(_ => _ == 0) > 0 || _cacheLevels.Length <= temp || _cacheLevels[temp] == 0)
                    {
                        MakeCacheLevel();
                    }
                    else
                    {
                        if (_cacheLevels != null && _cacheLevels.Length > 0)
                        {
                            var flagLevel = false;
                            for (int i = 0; i < _cacheLevels.Length; i++)
                            {
                                if (_cacheLevels[i] >= Config.MaxLevelCanReach)
                                {
                                    flagLevel = true;
                                    break;
                                }
                            }

                            if (flagLevel)
                            {
                                MakeCacheLevel();
                            }
                        }
                    }
                }

                var obj = await Addressables.LoadAssetAsync<GameObject>(string.Format(Constants.LEVEL_FORMAT, _cacheLevels[temp] + 1));
                return (obj, _cacheLevels[temp]);
            }

            var levelObject = await Addressables.LoadAssetAsync<GameObject>(string.Format(Constants.LEVEL_FORMAT, levelIndex + 1));

            return (levelObject, levelIndex);
        }

        #endregion
    }
}