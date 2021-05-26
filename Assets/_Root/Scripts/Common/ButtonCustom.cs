using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonCustom : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Button.ButtonClickedEvent onClick;

    private bool isMoveEnter = true;

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(.9f, 0.15f).SetEase(Ease.OutQuint);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.DOKill();
        transform.localScale = Vector3.one;
        if (isMoveEnter)
        {
            onClick.Invoke();
            SoundController.Instance.PlayOnce(SoundType.ButtonClick);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMoveEnter = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMoveEnter = false;
    }
}
