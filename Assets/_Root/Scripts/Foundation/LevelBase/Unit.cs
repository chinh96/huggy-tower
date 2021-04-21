using UnityEngine;

namespace Lance.TowerWar.LevelBase
{
    public abstract class Unit : MonoBehaviour, IUnit
    {
        public abstract void DarknessRise();

        public abstract void LightReturn();
    }
}