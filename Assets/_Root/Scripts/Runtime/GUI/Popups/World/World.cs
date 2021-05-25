using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class World : MonoBehaviour
{
    [SerializeField] private WorldType worldType;

    public WorldType WorldType { get => worldType; set => worldType = value; }

    [SerializeField] private List<Image> castles;

    private void OnEnable()
    {
        int index = 0;
        foreach (Image item in castles)
        {
            CastleData castleCurrent = UniverseResources.Instance.WorldCurrent.Castles[index].CastleCurrent;
            if (castleCurrent != null)
            {
                item.sprite = castleCurrent.Sprite;
                item.SetNativeSize();
            }
            else
            {
                item.gameObject.SetActive(false);
            }
        }
    }
}
