using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class NotiQuest : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI number;
    [SerializeField] private int startValueX = -270;
    [SerializeField] private int endValueX = 270;
    [SerializeField] private float duration = .3f;
    [SerializeField] private float delay = 2;

    public void Init(DailyQuestDayItem item)
    {
        image.sprite = item.Sprite;
        title.text = item.Title;
        number.text = item.Number;
    }

    public void MoveIn()
    {
        transform.DOLocalMoveX(endValueX, duration).OnComplete(() =>
        {
            DOTween.Sequence().AppendInterval(delay).AppendCallback(() =>
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
    }
}
