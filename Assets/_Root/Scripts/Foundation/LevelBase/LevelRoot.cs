using Lance.Common;
using UnityEngine;

namespace Lance.TowerWar.LevelBase
{
    public class LevelRoot : MonoBehaviour
    {
        #region properties

        [SerializeField, ReadOnly] private int levelIndex;
        [SerializeField, ReadOnly] private LevelMap levelMapPrefab;
        [SerializeField, ReadOnly] private LevelMap levelMap;
        public int LevelIndex => levelIndex;

        public LevelMap LevelMap => levelMap;
        public LevelMap LevelMapPrefab => levelMapPrefab;

        #endregion

        #region function

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
                levelMap = Instantiate(levelMapPrefab, transform, false);
                levelMap.transform.localPosition = Vector3.zero;
            }
        }

        /// <summary>
        /// enable
        /// </summary>
        public virtual void DarknessRise() { LevelMap.DarknessRise(); }

        /// <summary>
        /// disable
        /// </summary>
        public virtual void LightReturn() { LevelMap.LightReturn(); }

        /// <summary>
        /// clear level in root
        /// </summary>
        public virtual void Clear()
        {
            if (LevelMap != null) LevelMap.gameObject.Destroy();
        }

        #endregion
    }
}