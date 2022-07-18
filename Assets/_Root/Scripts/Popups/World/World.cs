using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class World : MonoBehaviour
{
    [SerializeField] private WorldType worldType;
    [SerializeField] private List<Image> castles;
    [SerializeField] private GameObject hammer;
    [SerializeField] private ParticleSystem smoke;
    [SerializeField] private Transform originalParentTransform;
    public WorldType WorldType { get => worldType; set => worldType = value; }
    
    public void ReturnOriginalPosition(){
        transform.SetParent(originalParentTransform, false);
        transform.SetSiblingIndex(0);
    }
    public void Reset()
    {
        //int index = 0;
        //foreach (Image castle in castles)
        //{
        //    CastleData castleCurrent = ResourcesController.Universe.WorldCurrent.Castles[index].CastleCurrent; // last isActive = true
        //    if (castleCurrent != null)
        //    {
        //        castle.sprite = castleCurrent.Sprite;
        //        castle.SetNativeSize();
        //        castle.gameObject.SetActive(true);
        //    }
        //    else
        //    {
        //        castle.gameObject.SetActive(false);
        //    }
        //    index++;
        //}
        //hammer.SetActive(false);
    }

    public void Build(int castleIndex)
    {
        SoundController.Instance.PlayOnce(SoundType.BuildItem);
        hammer.transform.position = castles[castleIndex].transform.position;
        hammer.transform.localPosition += new Vector3(100, 100, 0);
        hammer.SetActive(true);

        DOTween.Sequence().AppendInterval(2.2f).AppendCallback(() =>
        {
            Reset();
            smoke.transform.position = castles[castleIndex].transform.position;
            smoke.transform.localPosition += new Vector3(0, 100, 0);
            smoke.Play();
            SoundController.Instance.PlayOnce(SoundType.BuildItemDone);

            if (!Data.IsBuildFirstKingdomItem)
            {
                Data.IsBuildFirstKingdomItem = true;
                AnalyticController.BuildFirstKingdomItem();
            }
        });
    }
}
