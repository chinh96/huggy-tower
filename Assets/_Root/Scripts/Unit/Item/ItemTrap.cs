using TMPro;
using UnityEditor;
using UnityEngine;
using Spine.Unity;
using DG.Tweening;

public class ItemTrap : Item
{
    [SerializeField] private SkeletonGraphic skeleton;
    [SerializeField] private ParticleSystem particle;
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
                    DecreaseDamage(player, damage);
                    SoundController.Instance.PlayOnce(SoundType.BlockWallBreak);
                    break;
                case ItemType.Trap:
                    skeleton.Play("Attack", false);
                    DOTween.Sequence().AppendInterval(.2f).AppendCallback(() =>
                    {
                        DecreaseDamage(player, damage);
                    });
                    break;
                case ItemType.Bomb:
                    ParticleSystem particleSystem = Instantiate(particle, transform.parent);
                    particleSystem.transform.position = transform.position;
                    DecreaseDamage(player, player.Damage / 2);
                    break;
                case ItemType.Bow:
                    skeleton.Play("animation", false);
                    DOTween.Sequence().AppendInterval(.2f).AppendCallback(() =>
                    {
                        DecreaseDamage(player, damage);
                    });
                    break;
            }
        }
    }

    private void DecreaseDamage(Player player, int damage)
    {
        if (player.IncreaseDamage(-damage))
        {
            State = EUnitState.Invalid;

            switch (EquipType)
            {
                case ItemType.BrokenBrick:
                case ItemType.Bomb:
                    gameObject.SetActive(false);
                    break;
            }

            switch (EquipType)
            {
                case ItemType.BrokenBrick:
                    ResourcesController.DailyQuest.IncreaseByType(DailyQuestType.PushWall);
                    break;
            }

            txtDamage.gameObject.SetActive(false);
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