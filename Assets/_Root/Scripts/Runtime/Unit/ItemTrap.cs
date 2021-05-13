using Lance.TowerWar.LevelBase;
using TMPro;

namespace Lance.TowerWar.Unit
{
    public class ItemTrap : Item
    {
        public TextMeshProUGUI txtDamage;
        public int damage;

        public override void Collect(IUnit affectTarget)
        {
            if (State == EUnitState.Invalid)
            {
                return;
            }
        }
    }
}