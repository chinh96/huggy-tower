using TMPro;
using UnityEditor;
using UnityEngine;

public class ItemBrokenBrick : Item
{
    public TextMeshProUGUI txtDamage;
    public int damage;

    public override void Collect(IUnit affectTarget)
    {
        var player = (Player)affectTarget;
        if (player != null)
        {
            player.IncreaseDamage(-damage);
            gameObject.SetActive(false);
            SoundController.Instance.PlayOnce(SoundType.BlockWallBreak);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ItemBrokenBrick))]
public class ItemBrokenBrickEditor : UnityEditor.Editor
{
    private ItemBrokenBrick _item;

    private void OnEnable() { _item = (ItemBrokenBrick)target; }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        _item.txtDamage.text = $"-{_item.damage}";

        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();
    }
}
#endif