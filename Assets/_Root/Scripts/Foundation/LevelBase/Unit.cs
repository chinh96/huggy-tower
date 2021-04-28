using Lance.TowerWar.Unit;
using TMPro;
using UnityEngine;

namespace Lance.TowerWar.LevelBase
{
    public abstract class Unit : MonoBehaviour, IUnit, IAttack
    {
        public EUnitState state;
        public TextMeshProUGUI txtDamage;
        public int damage;
        public EUnitState State { get => state; set => state = value; }
        public virtual EUnitType Type { get; protected set; } = EUnitType.Enemy;
        public int Damage { get => damage; set => damage = value; }
        public TextMeshProUGUI TxtDamage => txtDamage;
        public abstract void BeingAttacked(bool attack, int damage);

        public abstract void DarknessRise();

        public abstract void LightReturn();
    }
}