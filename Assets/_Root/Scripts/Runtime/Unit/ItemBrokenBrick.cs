using Lance.TowerWar.LevelBase;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Lance.TowerWar.Unit
{
    public class ItemBrokenBrick : Item
    {
        public Collision2D coll2D;
        public TextMeshProUGUI txtDamage;
        public int damage;
        public override void Collect(IUnit affectTarget)
        {
            var player = (Player) affectTarget;
            if (player != null)
            {
                player.IncreaseDamage(-damage);
                // play effect
                var go = transform.GetChild(0);
                go.transform.SetParent(transform.parent, false);
                go.gameObject.SetActive(true);
                gameObject.SetActive(false);
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ItemBrokenBrick))]
    public class ItemBrokenBrickEditor : UnityEditor.Editor
    {
        private ItemBrokenBrick _item;

        private void OnEnable() { _item = (ItemBrokenBrick) target; }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            _item.txtDamage.text = $"-{_item.damage}";

            serializedObject.Update();
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}