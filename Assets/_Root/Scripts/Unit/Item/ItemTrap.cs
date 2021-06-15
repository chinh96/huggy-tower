using TMPro;
using UnityEditor;
using UnityEngine;
using Spine.Unity;
using DG.Tweening;

public class ItemTrap : Item
{
    [SerializeField] private SkeletonGraphic skeleton;
    public TextMeshProUGUI txtDamage;
    public int damage;

    public override void Collect(IUnit affectTarget)
    {
        var player = (Player)affectTarget;
        if (player != null)
        {
            switch (EquipType)
            {
                case ItemType.BrokenBrick:
                    DecreaseDamage(player);
                    SoundController.Instance.PlayOnce(SoundType.BlockWallBreak);
                    break;
                case ItemType.Trap:
                    skeleton.Play("Attack", false);
                    DOTween.Sequence().AppendInterval(.2f).AppendCallback(() =>
                    {
                        DecreaseDamage(player);
                    });
                    break;
            }
        }
    }

    private void DecreaseDamage(Player player)
    {
        if (player.IncreaseDamage(-damage))
        {
            State = EUnitState.Invalid;
            gameObject.SetActive(false);

            switch (EquipType)
            {
                case ItemType.BrokenBrick:
                    ResourcesController.DailyQuest.IncreaseByType(DailyQuestType.PushWall);
                    break;
            }
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ItemTrap))]
public class ItemBrokenBrickEditor : UnityEditor.Editor
{
    private ItemTrap _item;

    private void OnEnable() { _item = (ItemTrap)target; }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        _item.txtDamage.text = $"-{_item.damage}";

        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();
    }
}
#endif