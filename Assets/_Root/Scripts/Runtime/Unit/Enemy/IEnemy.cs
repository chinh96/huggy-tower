using TMPro;

namespace Lance.TowerWar.Unit
{
    public interface IAttack
    {
        int Damage { get; set; }
        TextMeshProUGUI TxtDamage { get; }

        void BeingAttacked(bool attack, int damage);
    }
}