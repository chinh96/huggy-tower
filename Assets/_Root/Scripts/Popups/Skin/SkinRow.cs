using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinRow : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private int maxItem = 3;
    [SerializeField] private SkinItem skinItem;

    private List<SkinItem> skinItems = new List<SkinItem>();

    public void Init(SkinResources skinResources, ref int index, SkinPopup skinPopup)
    {
        content.Clear();

        for (int i = 0; i < maxItem; i++)
        {
            if (index < skinResources.SkinDatas.Count)
            {
                SkinItem item = Instantiate(skinItem, content);
                item.Init(skinResources.SkinDatas[index], skinPopup, skinPopup.gameObject);
                skinItems.Add(item);
                index++;
            }
        }
    }

    public void Reset(SkinData skinData)
    {
        skinItems.ForEach(item => item.Reset());
    }

    public void ResetDock()
    {
        skinItems.ForEach(item => item.SetActiveDock(false));
    }

    public void ResetUsedLabel()
    {
        skinItems.ForEach(item => item.SetActiveUsedLabel(false));
    }

    public void ResetFx()
    {
        skinItems.ForEach(item => item.SetActiveFX(false));
    }
}
