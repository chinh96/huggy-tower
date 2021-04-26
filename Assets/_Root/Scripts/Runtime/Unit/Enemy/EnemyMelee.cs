namespace Lance.TowerWar.Unit
{
    using LevelBase;

    public class EnemyMelee : Unit, IEnemy
    {
        public override void DarknessRise() { }

        public override void LightReturn() { }
        public EUnitState State { get; set; }
    }
}