using Lance.TowerWar.LevelBase;
using UnityEngine;

namespace Lance.TowerWar.Unit
{
    public abstract class Item : MonoBehaviour, IUnit
    {
        public EUnitState State { get; set; }
        public virtual EUnitType Type { get; } = EUnitType.Item;
        public virtual void DarknessRise() {  }

        public virtual void LightReturn() {  }

        public abstract void Collect(IUnit affectTarget);
    }
}