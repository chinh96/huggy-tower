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

    private void Awake()
    {
        if (txtDamage != null && damage > 0)
        {
            txtDamage.DOCounter(-damage, -damage, 0);
        }
    }

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
                case ItemType.Electric:
                    DecreaseDamage(player, damage);
                    SoundController.Instance.PlayOnce(SoundType.BlockWallBreak);
                    break;
                case ItemType.Trap:
                    skeleton.Play("Attack", false);
                    DOTween.Sequence().AppendInterval(.2f).AppendCallback(() =>
                    {
                        Destroy(gameObject);
                        DecreaseDamage(player, damage);
                    });
                    SoundController.Instance.PlayOnce(SoundType.Trap);
                    break;
                case ItemType.Bomb:
                    ParticleSystem particleSystem = Instantiate(particle, transform.parent);
                    particleSystem.transform.position = transform.position;
                    DecreaseDamage(player, player.Damage / 2);
                    SoundController.Instance.PlayOnce(SoundType.Bomb);
                    break;
                case ItemType.Bow:
                    skeleton.Play("animation", false);
                    DOTween.Sequence().AppendInterval(.5f).AppendCallback(() =>
                    {
                        DecreaseDamage(player, damage);
                    });
                    SoundController.Instance.PlayOnce(SoundType.Bow);
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
                case ItemType.Electric:
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

        if (_item.EquipType != ItemType.Bomb)
        {
            _item.txtDamage.text = $"-{_item.damage}";

            serializedObject.Update();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif