using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Lance.Common;
using UnityEngine;
using UnityEngine.AddressableAssets;

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
        if (Data.CurrentLevel > Config.Instance.MaxLevelCanReach)
        {
            _cacheLevels = new int[Config.Instance.MaxLevelWithOutTutorial];

            for (int i = 0; i < Config.Instance.MaxLevelWithOutTutorial; i++)
            {
                _cacheLevels[i] = Data.GetCacheLevelIndex(i);
                if (_cacheLevels[i] >= Config.Instance.MaxLevelCanReach) flagOutLevel = true;
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
        for (int i = 0; i < Config.Instance.MaxLevelCanReach; i++)
        {
            if (Config.Instance.LevelSkips.Contains(i)) continue;
            tempList.Add(i);
        }

        tempList.Shuffle();

        _cacheLevels = new int[Config.Instance.MaxLevelWithOutTutorial];

        for (int i = 0; i < tempList.Count; i++)
        {
            Data.SetCacheLevelIndex(i, tempList[i]);
            _cacheLevels[i] = tempList[i];
        }
    }

    /// <summary>
    /// return level prefab and RealLevelIndex, FakeLevelIndex
    /// </summary>
    /// <param name="levelIndex"></param>
    /// <returns></returns>
    public async UniTask<(GameObject, int, int)> GetLevel(int levelIndex)
    {
        if (levelIndex > Config.Instance.MaxLevelCanReach - 1)
        {
            var temp = (levelIndex - Config.Instance.MaxLevelCanReach) % (Config.Instance.MaxLevelWithOutTutorial);
            if (Data.CountPlayLevel >= Config.Instance.MaxLevelWithOutTutorial)
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
                            if (_cacheLevels[i] >= Config.Instance.MaxLevelCanReach)
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
            //Debug.Log("realIndex:" + _cacheLevels[temp] + "       fakeIndex:" + levelIndex);
            return (obj, _cacheLevels[temp], levelIndex);
        }

        var levelObject = await Addressables.LoadAssetAsync<GameObject>(string.Format(Constants.LEVEL_FORMAT, levelIndex + 1));

        //Debug.Log("realIndex:" + levelIndex + "       fakeIndex:" + levelIndex);
        return (levelObject, levelIndex, levelIndex);
    }

    #endregion
}