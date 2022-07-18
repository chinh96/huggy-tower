using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Purchasing;


public class DataBridge : Singleton<DataBridge>
{
    [SerializeField] private IAPManager iapManager;
    public LevelMap PreviousLevelLoaded;
    public LevelMap NextLevelLoaded;

    private int[] _cacheLevels;
    private int _countGetLevel = 0;
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        // CheckCacheLevel(); Old

        // InitialzieIAP(); PhucVH
    }

    private void InitialzieIAP()
    {
        if (!iapManager.IsInitialize)
        {
            iapManager.Initialized(new[]
            {
                    new IAPData(Constants.IAP_PACK1, ProductType.Consumable.ToString()), new IAPData(Constants.IAP_PACK2, ProductType.Consumable.ToString()),
                    new IAPData(Constants.IAP_PACK3, ProductType.Consumable.ToString()), new IAPData(Constants.IAP_REMOVE_ADS, ProductType.NonConsumable.ToString()),
                    new IAPData(Constants.IAP_UNLOCK_HERO, ProductType.NonConsumable.ToString()), new IAPData(Constants.IAP_VIP, ProductType.NonConsumable.ToString()),
            });
        }

    }

    public void CheckCacheLevel()
    {
        bool flagOutLevel = false;
        if (Data.CurrentLevel > ResourcesController.Config.MaxLevelCanReach)
        {
            _cacheLevels = new int[ResourcesController.Config.MaxLevelWithOutTutorial];

            for (int i = 0; i < ResourcesController.Config.MaxLevelWithOutTutorial; i++)
            {
                _cacheLevels[i] = Data.GetCacheLevelIndex(i);
                if (_cacheLevels[i] >= ResourcesController.Config.MaxLevelCanReach) flagOutLevel = true;
            }

            if (flagOutLevel) MakeCacheLevel();
        }
    }

    public void MakeCacheLevel()
    {
        var tempList = new List<int>();
        for (int i = 0; i < ResourcesController.Config.MaxLevelCanReach; i++)
        {
            if (ResourcesController.Config.LevelSkips.Contains(i)) continue;
            tempList.Add(i);
        }

        tempList.Shuffle();

        _cacheLevels = new int[ResourcesController.Config.MaxLevelWithOutTutorial];

        for (int i = 0; i < tempList.Count; i++)
        {
            Data.SetCacheLevelIndex(i, tempList[i]);
            _cacheLevels[i] = tempList[i];
        }
    }

    public async Task<(GameObject, int, int)> GetLevel(int levelIndex)
    {
        // if (levelIndex > ResourcesController.Config.MaxLevelCanReach - 1)
        // {
        //     var temp = (levelIndex - ResourcesController.Config.MaxLevelCanReach) % (ResourcesController.Config.MaxLevelWithOutTutorial);
        //     if (Data.CountPlayLevel >= ResourcesController.Config.MaxLevelWithOutTutorial)
        //     {
        //         MakeCacheLevel();
        //         Data.CountPlayLevel = 0;
        //     }
        //     else
        //     {
        //         if (_cacheLevels == null || _cacheLevels.Length == 0 || _cacheLevels.Count(_ => _ == 0) > 0 || _cacheLevels.Length <= temp || _cacheLevels[temp] == 0)
        //         {
        //             MakeCacheLevel();
        //         }
        //         else
        //         {
        //             if (_cacheLevels != null && _cacheLevels.Length > 0)
        //             {
        //                 var flagLevel = false;
        //                 for (int i = 0; i < _cacheLevels.Length; i++)
        //                 {
        //                     if (_cacheLevels[i] >= ResourcesController.Config.MaxLevelCanReach)
        //                     {
        //                         flagLevel = true;
        //                         break;
        //                     }
        //                 }

        //                 if (flagLevel)
        //                 {
        //                     MakeCacheLevel();
        //                 }
        //             }
        //         }
        //     }

        //     var obj = await Addressables.LoadAssetAsync<GameObject>(string.Format(Constants.LEVEL_FORMAT, _cacheLevels[temp] + 1)).Task;
        //     return (obj, _cacheLevels[temp], levelIndex);
        // }
        _countGetLevel ++;
        if (levelIndex > ConfigResources.MaxLevel - 1)
        {
            if(Data.CurrentLoopLevel == -1 || (Data.CurrentLoopLevel != -1 && Data.IsWinCurrentLoopLevel) ){
                levelIndex = Random.Range(0, ConfigResources.MaxLevel);
                Data.CurrentLoopLevel = levelIndex;
                Data.IsWinCurrentLoopLevel = false;
            }
            else
            {
                if(_countGetLevel != 1) levelIndex = Random.Range(0, ResourcesController.Config.MaxLevelCanReach);
                else levelIndex = Data.CurrentLoopLevel;
            }
        }
        var levelObject = await Addressables.LoadAssetAsync<GameObject>(string.Format(Constants.LEVEL_FORMAT, levelIndex + 1)).Task;

        return (levelObject, levelIndex, levelIndex);
    }
}