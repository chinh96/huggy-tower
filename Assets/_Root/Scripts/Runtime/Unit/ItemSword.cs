using Lance.TowerWar.LevelBase;
using TMPro;
using UnityEditor;
using UnityEngine.UI;

namespace Lance.TowerWar.Unit
{
    using UnityEngine;

    public class ItemSword : Item
    {
        public int indexSkin = 0;
        public Image render;
        public Rigidbody2D rigid;
        public Collider2D coll2D;
        public TextMeshProUGUI txtDamage;
        public int damage;

        public override void Collect(IUnit affectTarget)
        {
            var player = (Player) affectTarget;
            if (player != null)
            {
                gameObject.SetActive(false);
                player.isUsingSword = true;
                player.IncreaseDamage(damage);
                player.MixAndMatchSkin.Refresh(indexSkin);
                // play effect
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ItemSword))]
    public class ItemSwordEditor : UnityEditor.Editor
    {
        private ItemSword _item;

        private void OnEnable() { _item = (ItemSword) target; }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            _item.txtDamage.text = $"+{_item.damage}";

            serializedObject.Update();
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}