using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class DataBridge : Singleton<DataBridge>
{
    public LevelMap PreviousLevelLoaded;
    public LevelMap NextLevelLoaded;

    private int[] _cacheLevels;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        CheckCacheLevel();
    }

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

    public async Task<(GameObject, int, int)> GetLevel(int levelIndex)
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

            var obj = await Addressables.LoadAssetAsync<GameObject>(string.Format(Constants.LEVEL_FORMAT, _cacheLevels[temp] + 1)).Task;
            return (obj, _cacheLevels[temp], levelIndex);
        }

        var levelObject = await Addressables.LoadAssetAsync<GameObject>(string.Format(Constants.LEVEL_FORMAT, levelIndex + 1)).Task;

        return (levelObject, levelIndex, levelIndex);
    }
}