using Lance.Common;
using UnityEngine;

namespace Lance.TowerWar.LevelBase
{
    public class LevelRoot : MonoBehaviour
    {
        #region properties

        [SerializeField, ReadOnly] private int levelIndex;
        [SerializeField, ReadOnly] private LevelMap levelMapPrefab;

        public int LevelIndex => levelIndex;

        public LevelMap LevelMap { get; private set; }

        #endregion

        #region function

        public virtual void Initialized(int level, LevelMap prefab)
        {
            levelIndex = level;
            levelMapPrefab = prefab;
        }

        public virtual void Install()
        {
            if (LevelMap != null)
            {
                DestroyImmediate(LevelMap);
            }

            LevelMap = Instantiate(levelMapPrefab, transform);
            var levelMap = LevelMap.transform;
            levelMap.localPosition = Vector3.zero;
            levelMap.localScale = Vector3.one;
            levelMap.localEulerAngles = Vector3.zero;
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