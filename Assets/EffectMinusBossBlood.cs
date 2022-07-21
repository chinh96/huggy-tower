using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
public class EffectMinusBossBlood : MonoBehaviour
{
    private TextMeshProUGUI _text;
    private RectTransform _rectTransform;
    private Vector3 _originalPos;
    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
        _rectTransform = GetComponent<RectTransform>();
        _originalPos = _rectTransform.localPosition;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnEnable()
    {
        _text.color = new Color(1, 0, 0);
        _rectTransform.localPosition = _originalPos;
        Sequence _sequence = DOTween.Sequence();
        _sequence.Append(_rectTransform.DOLocalMoveY(_rectTransform.localPosition.y + 300, .6f));
        _sequence.Join(_text.DOColor(new Color(1, 0.5f, 0.5f), .6f)).AppendCallback(() => { gameObject.SetActive(false);});
    }
}
