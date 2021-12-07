using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonCustom : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Button.ButtonClickedEvent onClick;

    public bool canClick = true;
    private bool isMoveEnter = false;

    private Vector3 oldScale = new Vector3();
    public void OnPointerDown(PointerEventData eventData)
    {
        if (canClick)
        {
            transform.DOScale(.9f, .01f).SetEase(Ease.OutQuint);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (canClick)
        {
            transform.localScale = oldScale;
            if (isMoveEnter)
            {
                onClick.Invoke();
                SoundController.Instance.PlayOnce(SoundType.ButtonClick);
            }
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
    private void Start()
    {
        oldScale = transform.localScale;
    }
}
