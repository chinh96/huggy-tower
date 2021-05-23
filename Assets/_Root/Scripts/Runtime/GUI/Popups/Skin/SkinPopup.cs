using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinPopup : Popup
{
    [SerializeField] private EUnitType eUnitType;
    [SerializeField] private Transform content;
    [SerializeField] private SkinRow skinRow;

    private SkinResources skinResources;

    private List<SkinRow> skinRows = new List<SkinRow>();

    public EUnitType EUnitType { get => eUnitType; set => eUnitType = value; }

    private void Awake()
    {
        switch (eUnitType)
        {
            case EUnitType.Hero:
                skinResources = ResourcesController.Instance.Hero;
                break;
            case EUnitType.Princess:
                skinResources = ResourcesController.Instance.Princess;
                break;
        }
    }

    protected override void AfterInstantiate()
    {
        base.AfterInstantiate();

        content.Clear();

        int index = 0;
        while (index < skinResources.SkinDatas.Count)
        {
            SkinRow row = Instantiate(skinRow, content);
            row.Init(skinResources, ref index, this);
            skinRows.Add(row);
        }
    }

    public void Reset()
    {
        int index = 0;
        skinRows.ForEach(item =>
        {
            item.Reset(skinResources.SkinDatas[index]);
            index++;
        });
    }
}
