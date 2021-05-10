using Lance.Common;
using Lance.TowerWar.LevelBase;
using Spine.Unity;
using UnityEngine;

namespace Lance.TowerWar.Unit
{
    public class ItemChest : Item, IAnim
    {
        [SerializeField] private SkeletonGraphic skeleton;
        public Rigidbody2D rigid;
        public Collider2D coll2D;

        public SkeletonGraphic Skeleton => skeleton;
        public void PlayIdle(bool isLoop) { }

        public void PlayAttack() { }

        public void PLayMove(bool isLoop) { }

        public void PlayDead() { }

        public void PlayWin(bool isLoop = false) { skeleton.Play("animation", isLoop); }

        public void PlayLose(bool isLoop = false) { skeleton.Play("Fire", isLoop); }

        public override void Collect(IUnit affectTarget) { PlayWin(); }
    }
}