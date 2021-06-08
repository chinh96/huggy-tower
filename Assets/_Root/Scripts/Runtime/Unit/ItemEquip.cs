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
            player.IncreaseDamage(damage);
            player.ChangeSword(itemSwordSkin);
            SoundController.Instance.PlayOnce(SoundType.HeroPickSword);
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

        _item.txtDamage.text = $"+{_item.damage}";

        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();
    }
}
#endif