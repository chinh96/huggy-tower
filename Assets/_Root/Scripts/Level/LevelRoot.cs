using UnityEngine;

public class LevelRoot : MonoBehaviour
{
    [SerializeField] private int levelIndex;
    [SerializeField] private LevelMap levelMapPrefab;
    [SerializeField] private LevelMap levelMap;
    [SerializeField] private FighterOverlay fighterOverlay;

    private int times = 0;
    private int totalTimesPlay = 0;
    private int totalLevelWin = 0;
    private int totalTimesLose = 0;
    public int LevelIndex => levelIndex;
    public LevelMap LevelMap => levelMap;
    public LevelMap LevelMapPrefab => levelMapPrefab;

    public virtual void Initialized(int level, LevelMap levelMapPrefab)
    {
        levelIndex = level;
        this.levelMapPrefab = levelMapPrefab;
    }

    public void Install()
    {
        Clear();
        if (levelMapPrefab != null)
        {
#if UNITY_EDITOR
            //ResourcesController.Config.LevelDebug = null;
            if (ResourcesController.Config.LevelDebug != null)
            {
                levelMap = Instantiate(ResourcesController.Config.LevelDebug, transform, false);
            }
            else
            {
                levelMap = Instantiate(levelMapPrefab, transform, false);
                levelMap.transform.localPosition = Vector3.zero;
            }
#else
            levelMap = Instantiate(levelMapPrefab, transform, false);
            levelMap.transform.localPosition = Vector3.zero;
#endif
            // Instantiate(fighterOverlay, levelMap.transform);
            StartTimer();
        }
    }

    public virtual void DarknessRise() { LevelMap.DarknessRise(); }

    public virtual void LightReturn() { LevelMap.LightReturn(); }

    public virtual void Clear()
    {
        if (LevelMap != null)
        {
            Destroy(LevelMap.gameObject);
        }
        IncreaseTotalLevelWin();
        StopTimer();
    }

    public void StartTimer()
    {
        times = 0;
        InvokeRepeating("IncreaseTimes", 0, 1);
    }

    public void StopTimer()
    {
        totalTimesPlay += times;
        CancelInvoke("IncreaseTimes");
    }

    private void IncreaseTimes()
    {
        times++;
        totalTimesLose++;
    }

    public void ResetTotalTimesPlay()
    {
        totalTimesPlay = 0;

    }
    public void RestTotalTimesLose()
    {
        totalTimesLose = 0;
    }

    public int GetTotalTimesPlay()
    {
        return totalTimesPlay;
    }
    public int GetTotalTimesLose()
    {
        return totalTimesLose;
    }

    public void IncreaseTotalLevelWin()
    {
        totalLevelWin++;
    }

    public int GetTotalLevelWin()
    {
        return totalLevelWin;
    }

    public void ResetTotalLevelWin()
    {
        totalLevelWin = 0;
    }
}