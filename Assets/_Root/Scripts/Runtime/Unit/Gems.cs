using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

namespace Lance.TowerWar.Unit
{
    using UnityEngine;

    public class Gems : MonoBehaviour
    {
        private Rigidbody2D _rigid;
        private CircleCollider2D _coll;
        private TweenerCore<Vector3, Vector3, VectorOptions> _tween;


        private void Start()
        {
            _rigid = GetComponent<Rigidbody2D>();
            _coll = GetComponent<CircleCollider2D>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="duration"></param>
        public void CollectByPlayer(Transform parent, float duration = 0.5f)
        {
            _rigid.simulated = false;
            _coll.enabled = false;
            transform.SetParent(parent, true);
            _tween = transform.DOLocalMove(Vector3.zero, duration)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    try
                    {
                        gameObject.SetActive(false);
                    }
                    catch
                    {
                        //
                    }
                });
        }

        public void Dispose() { DOTween.Kill(_tween); }
    }
}