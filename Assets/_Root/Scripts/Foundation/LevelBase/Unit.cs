using System;
using Lance.TowerWar.Unit;
using TMPro;
using UnityEngine;

namespace Lance.TowerWar.LevelBase
{
    public abstract class Unit : MonoBehaviour, IUnit, IAttack
    {
        [SerializeField] protected EUnitState state;
        public TextMeshProUGUI txtDamage;
        public int damage;
        public EUnitState State { get => state; set => state = value; }
        public virtual EUnitType Type { get; protected set; } = EUnitType.Enemy;
        public int Damage { get => damage; set => damage = value; }
        public TextMeshProUGUI TxtDamage => txtDamage;
        public abstract void OnBeingAttacked();

        public abstract void OnAttack(int damage, Action callback);

        public abstract void DarknessRise();

        public abstract void LightReturn();
    }
}