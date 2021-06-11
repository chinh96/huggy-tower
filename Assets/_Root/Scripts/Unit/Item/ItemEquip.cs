using TMPro;
using UnityEditor;
using UnityEngine.UI;

using UnityEngine;
using Spine.Unity;

public class ItemEquip : Item, IHasSkeletonDataAsset
{
    [SerializeField] private SkeletonDataAsset skeletonDataAsset;
    public SkeletonDataAsset SkeletonDataAsset => skeletonDataAsset;
    [SerializeField, SpineSkin] private string itemSwordSkin;

    public Image render;
    public Rigidbody2D rigid;
    public Collider2D coll2D;
    public TextMeshProUGUI txtDamage;
    public int damage;

    public void AddJumpAnimation()
    {
        render.gameObject.AddComponent<JumpAnimation>();
    }

    public override void Collect(IUnit affectTarget)
    {
        var player = (Player)affectTarget;
        if (player != null)
        {
            State = EUnitState.Invalid;
            gameObject.SetActive(false);
            player.EquipType = EquipType;
            IncreaseDamage(player);
            ChangeSword(player);
        }
    }

    private void IncreaseDamage(Player player)
    {
        if (EquipType != ItemType.Key)
        {
            int damage = this.damage;

            switch (EquipType)
            {
                case ItemType.Food:
                    damage = player.Damage;
                    break;
            }

            player.IncreaseDamage(damage);
        }
    }

    private void ChangeSword(Player player)
    {
        if (EquipType != ItemType.Food && EquipType != ItemType.Shield)
        {
            player.ChangeSword(itemSwordSkin);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ItemEquip))]
public class ItemSwordEditor : UnityEditor.Editor
{
    private ItemEquip _item;

    private void OnEnable() { _item = (ItemEquip)target; }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (_item.EquipType != ItemType.Food && _item.EquipType != ItemType.Key)
        {
            _item.txtDamage.text = $"+{_item.damage}";

            serializedObject.Update();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif