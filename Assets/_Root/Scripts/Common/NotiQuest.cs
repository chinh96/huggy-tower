using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using I2.Loc;

public class NotiQuest : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI number;
    [SerializeField] private int startValueX = -270;
    [SerializeField] private int endValueX = 270;
    [SerializeField] private float duration = .3f;
    [SerializeField] private float delay = 2;

    private Sequence sequence;

    public void Init(DailyQuestDayItem item)
    {
        image.sprite = item.Sprite;
        //title.text = item.Title;
        title.GetComponent<Localize>().SetTerm("DailyQuestItem_txt" + item.Type + "Type");
        title.GetComponent<LocalizationParamsManager>().SetParameterValue("VALUE", item.Number, true);
        number.text = item.Number;
    }

    public void MoveIn()
    {
        transform.DOLocalMoveX(endValueX, duration).OnComplete(() =>
        {
            sequence = DOTween.Sequence().AppendInterval(delay).AppendCallback(() =>
            {
                MoveOut();
            });
        });
    }

    public void MoveOut()
    {
        transform.DOLocalMoveX(startValueX, duration).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }

    public void OnClick()
    {
        PopupController.Instance.Show<AchievementDailyQuestPopup>();
        MoveOut();
    }

    private void OnDestroy()
    {
        transform.DOKill();
        sequence.Kill();
    }
}
