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


    public override void Collect(IUnit affectTarget)
    {
        var player = (Player)affectTarget;
        if (player != null)
        {
            gameObject.SetActive(false);
            player.EquipType = EquipType;
            IncreaseDamage(player);
            ChangeSword(player);
            SoundController.Instance.PlayOnce(SoundType.HeroPickSword);
        }
    }

    private void IncreaseDamage(Player player)
    {
        player.IncreaseDamage(GetDamage(player));
    }

    private void ChangeSword(Player player)
    {
        if (EquipType != ItemType.Food)
        {
            player.ChangeSword(itemSwordSkin);
        }
    }

    private int GetDamage(Player player)
    {
        int damage = this.damage;

        switch (EquipType)
        {
            case ItemType.Food:
                damage = player.Damage;
                break;
        }

        return damage;
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

        if (_item.EquipType != ItemType.Food)
        {
            _item.txtDamage.text = $"+{_item.damage}";

            serializedObject.Update();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif