using TMPro;
using UnityEditor;
using UnityEngine.UI;

using UnityEngine;
using Spine.Unity;

public class ItemSword : Item, IHasSkeletonDataAsset
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
            player.isUsingSword = true;
            player.IncreaseDamage(damage);
            player.ChangeSword(itemSwordSkin);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ItemSword))]
public class ItemSwordEditor : UnityEditor.Editor
{
    private ItemSword _item;

    private void OnEnable() { _item = (ItemSword)target; }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        _item.txtDamage.text = $"+{_item.damage}";

        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();
    }
}
#endif