using TMPro;
using UnityEditor;
using UnityEngine.UI;
using DG.Tweening;
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

    private WavyAnimation wavyAnimation;

    private void Awake()
    {
        if (txtDamage != null && damage > 0)
        {
            txtDamage.DOCounter(damage, damage, 0);
        }

        DOTween.Sequence().AppendInterval(Random.Range(0, .5f)).AppendCallback(() =>
        {
            wavyAnimation = render.gameObject.AddComponent<WavyAnimation>();
        });
    }

    public void AddJumpAnimation()
    {
        Destroy(wavyAnimation);
        render.gameObject.AddComponent<JumpAnimation>();
    }

    public override void Collect(IUnit affectTarget)
    {
        var player = (Player)affectTarget;
        if (player != null)
        {
            State = EUnitState.Invalid;
            gameObject.SetActive(false);
            switch (EquipType)
            {
                case ItemType.Sword:
                case ItemType.Gloves:
                case ItemType.Knife:
                case ItemType.Axe:
                    player.EquipType = EquipType;
                    break;
            }
            IncreaseDamage(player);
            ChangeSword(player);
            CheckDailyQuest();
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
                    damage = player.Damage / 2;
                    break;
                case ItemType.HolyWater:
                    damage = player.Damage;
                    break;
            }

            player.IncreaseDamage(damage);
        }
    }

    private void ChangeSword(Player player)
    {
        if (EquipType == ItemType.Key && GameController.Instance.ItemLock != null)
        {
            return;
        }

        if (EquipType != ItemType.Food && EquipType != ItemType.HolyWater)
        {
            player.ChangeSword(itemSwordSkin);
        }
    }

    private void CheckDailyQuest()
    {
        switch (EquipType)
        {
            case ItemType.Shield:
                ResourcesController.DailyQuest.IncreaseByType(DailyQuestType.GetShield);
                break;
            case ItemType.Food:
                ResourcesController.DailyQuest.IncreaseByType(DailyQuestType.GetFood);
                break;
            case ItemType.Sword:
                ResourcesController.DailyQuest.IncreaseByType(DailyQuestType.GetSword);
                break;
            case ItemType.Gloves:
                ResourcesController.DailyQuest.IncreaseByType(DailyQuestType.GetGloves);
                break;
            case ItemType.HolyWater:
                ResourcesController.DailyQuest.IncreaseByType(DailyQuestType.GetHolyWater);
                break;
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

        if (_item.EquipType != ItemType.Food && _item.EquipType != ItemType.Key && _item.EquipType != ItemType.HolyWater)
        {
            _item.txtDamage.text = $"+{_item.damage}";

            serializedObject.Update();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif