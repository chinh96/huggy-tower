using System;
using TMPro;

namespace Lance.TowerWar.Unit
{
    public interface IAttack
    {
        int Damage { get; set; }
        TextMeshProUGUI TxtDamage { get; }
        void OnBeingAttacked();
        void OnAttack(int damage, Action callback);
    }
}