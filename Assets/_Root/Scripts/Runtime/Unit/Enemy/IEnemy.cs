using TMPro;

namespace Lance.TowerWar.Unit
{
    public interface IEnemy
    {
        int Damage { get; set; }
        EUnitState State { get; set; }
        TextMeshProUGUI TxtDamage { get; }

        void BeingAttacked(bool attack, int damage);
    }
}