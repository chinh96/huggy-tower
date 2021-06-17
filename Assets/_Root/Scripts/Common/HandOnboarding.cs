using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Spine.Unity;
using Spine;
using UnityEngine.EventSystems;

public class HandOnboarding : MonoBehaviour, IHasSkeletonDataAsset, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private SkeletonDataAsset skeletonDataAsset;
    public SkeletonDataAsset SkeletonDataAsset => skeletonDataAsset;
    public RectTransform StartObject;
    public RectTransform EndObject;
    public SkeletonGraphic HandObject;
    [SpineAnimation] public string HandAnim;
    public float duration = 1;
    public ParticleSystem FX;
    public RectTransform Arrows;
    public GameObject Round1;
    public GameObject Round2;
    public GameObject TextBackgroundHero;
    public GameObject TextBackgroundEnemy;
    public GameObject Arrow;
    public GameObject Text2;
    public GameObject OKButton;

    private bool isMouseDowned;
    private bool isMouseUped;
    private Vector2 sizeDelta;
    private Sequence sequence;

    private void Awake()
    {
        if (Data.DoneOnboarding)
        {
            Destroy(gameObject);
        }
        else
        {
            Round1.SetActive(true);
            Round2.SetActive(false);
            sizeDelta = Arrows.sizeDelta;
            Arrows.sizeDelta = Vector2.zero;
            Reset();
        }
    }

    private void Start()
    {
        GameController.Instance.IsOnboarding = true;
        BeforeMove();
    }

    private void Reset()
    {
        HandObject.transform.position = StartObject.position;
    }

    private void BeforeMove()
    {
        HandObject.Play(HandAnim, false);
        HandObject.AnimationState.Complete += DoneHandAnim;
    }

    private void DoneHandAnim(TrackEntry args)
    {
        HandObject.AnimationState.Complete -= DoneHandAnim;
        FX.Play();
        sequence = DOTween.Sequence().AppendInterval(FX.main.duration).AppendCallback(() =>
        {
            Move();
        });
    }

    private void Move()
    {
        HandObject.transform.DOMove(EndObject.position, duration).OnComplete(EndMove);
        Arrows.DOSizeDelta(sizeDelta, duration).OnComplete(() =>
        {
            Arrows.sizeDelta = Vector2.zero;
        });
    }

    private void EndMove()
    {
        Reset();
        BeforeMove();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isMouseDowned = StartObject.Contains(Input.mousePosition, Camera.main);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isMouseUped = EndObject.Contains(Input.mousePosition, Camera.main);
        if (isMouseDowned && isMouseUped)
        {
            ShowRound2();
        }
        else
        {
            isMouseDowned = false;
        }
    }

    private void ShowRound2()
    {
        sequence.Kill();
        HandObject.transform.DOKill();
        Arrows.DOKill();

        Round1.SetActive(false);
        Round2.SetActive(true);

        TextBackgroundHero.SetActive(false);
        TextBackgroundEnemy.SetActive(false);
        Arrow.SetActive(false);
        Text2.SetActive(false);
        OKButton.SetActive(false);

        DOTween.Sequence().AppendInterval(.3f).AppendCallback(() =>
        {
            TextBackgroundHero.SetActive(true);
            DOTween.Sequence().AppendInterval(.5f).AppendCallback(() =>
            {
                Arrow.SetActive(true);
                DOTween.Sequence().AppendInterval(.5f).AppendCallback(() =>
                {
                    TextBackgroundEnemy.SetActive(true);
                    DOTween.Sequence().AppendInterval(1.5f).AppendCallback(() =>
                    {
                        Text2.SetActive(true);
                        DOTween.Sequence().AppendInterval(.5f).AppendCallback(() =>
                        {
                            OKButton.SetActive(true);
                        });
                    });
                });
            });
        });
    }

    public void OnClickOKButton()
    {
        Data.DoneOnboarding = true;
        GameController.Instance.IsOnboarding = false;
        Destroy(gameObject);
    }
}
