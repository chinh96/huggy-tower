using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonCustom : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] UnityEngine.UI.Button.ButtonClickedEvent onClick;

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
