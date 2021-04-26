namespace Lance.TowerWar.Unit
{
    using LevelBase;

    public class EnemyRange : Unit, IEnemy
    {
        public override void DarknessRise() { }

        public override void LightReturn() { }
        public EUnitState State { get; set; }
    }
}