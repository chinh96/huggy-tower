using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using DG.Tweening;
public class HeroWinLoseController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private SkeletonGraphic character;

    public void PlayWin(){
        string[] winList = {"Win", "Win2"};
        string win = winList[UnityEngine.Random.Range(0, winList.Length)];
        // float timeDelay = 0.6f;
        // if(win == "Win") timeDelay = 1.6f;
        // DOTween.Sequence().AppendInterval(timeDelay).AppendCallback(()=> character.Play("Idle", true));
        character.Play(win, true);
    }

    public void PlayLose(){

    }
}
