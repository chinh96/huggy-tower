using Lance.Common;
using Spine.Unity;
using TMPro;

namespace Lance.TowerWar.Unit
{
    using LevelBase;

    public class EnemyRange : Unit, IEnemy, IAnim
    {
        public SkeletonGraphic skeleton;
        public int damage;
        public TextMeshProUGUI txtDamage;
        public override void DarknessRise() { }

        public override void LightReturn() { }
        
        public EUnitState State { get; set; }
        public int Damage { get => damage; set => damage = value; }
        public TextMeshProUGUI TxtDamage => txtDamage;

        public void BeingAttacked(bool attack, int damage)
        {
            
        }

        public SkeletonGraphic Skeleton => skeleton;
        public void PlayIdle(bool isLoop) { skeleton.Play("idle", true); }

        public void PlayAttack() { skeleton.Play("attack (cung)", false); }

        public void PLayMove(bool isLoop) { skeleton.Play("run", true); }

        public void PlayDead() { skeleton.Play("die", false); }

        public void PlayWin(bool isLoop) { }

        public void PlayLose(bool isLoop) { }
    }
}