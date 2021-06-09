using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas), typeof(CanvasGroup), typeof(GraphicRaycaster))]
public class Popup : MonoBehaviour
{
    [SerializeField] protected Transform container;
    [SerializeField] float duration = 0.2f;
    [SerializeField] Ease ease = Ease.OutBack;
    [SerializeField] private bool isAnimScaleDown = false;
    public ShowAction showAction;

    Canvas canvas;
    CanvasGroup canvasGroup;
    PopupController controller;

    public object data;

    internal void Initialize(PopupController c)
    {
        canvas = GetComponent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        controller = c;

        canvasGroup.alpha = 0;
        container.localScale = Vector3.zero;

        AfterInstantiate();
    }

    internal void SetOrder(int order)
    {
        canvas.sortingOrder = order;
    }

    public virtual void Close()
    {
        controller.Dismiss(this);
    }

    internal virtual void Show(object data = null)
    {
        this.data = data;
        BeforeShow();
        ShowAnim(AfterShown);
    }

    internal void Resume(object data = null)
    {
        BeforeResume();
        ShowAnim(AfterResumed, false);
    }

    internal void Dismiss(bool animated)
    {
        BeforeDismiss();
        HideAnim(() =>
        {
            gameObject.SetActive(false);
            AfterDismissed();
        });
    }

    internal void Pause(bool animated)
    {
        BeforePause();
        HideAnim(AfterPaused);
    }

    void ShowAnim(System.Action onCompleted = null, bool isShowAnim = true)
    {
        DOTween.Kill(this, true);

        canvasGroup.interactable = false;
        gameObject.SetActive(true);

        if (isShowAnim)
        {
            if (isAnimScaleDown)
            {
                container.localScale = Vector3.one * 2;
                canvasGroup.alpha = 1;
            }
            else
            {
                DOTween.To(() => canvasGroup.alpha, x => { canvasGroup.alpha = x; }, 1, duration)
                    .SetUpdate(true).SetTarget(this);
            }

            container.DOScale(Vector3.one, duration).SetEase(ease).SetUpdate(true).SetTarget(this).OnComplete(
                () =>
                {
                    canvasGroup.alpha = 1;
                    container.localScale = Vector3.one;
                    canvasGroup.interactable = true;
                    onCompleted?.Invoke();
                });
        }
        else
        {
            canvasGroup.alpha = 1;
            container.localScale = Vector3.one;
            canvasGroup.interactable = true;
            onCompleted?.Invoke();
        }
    }

    void HideAnim(System.Action onCompleted)
    {
        DOTween.Kill(this, true);

        canvasGroup.interactable = false;

        canvasGroup.alpha = 0;
        container.localScale = Vector3.zero;
        onCompleted?.Invoke();
    }

    protected virtual void AfterInstantiate()
    {
    }

    protected virtual void BeforeShow()
    {
    }

    protected virtual void AfterShown()
    {
    }

    protected virtual void BeforeDismiss()
    {
    }

    protected virtual void AfterDismissed()
    {
    }

    protected virtual void BeforePause()
    {
    }

    protected virtual void AfterPaused()
    {
    }

    protected virtual void BeforeResume()
    {
    }

    protected virtual void AfterResumed()
    {
    }
}
