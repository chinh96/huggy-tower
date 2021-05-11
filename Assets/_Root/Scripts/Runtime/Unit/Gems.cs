using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

namespace Lance.TowerWar.Unit
{
    using UnityEngine;

    public class Gems : MonoBehaviour
    {
        private TweenerCore<Vector3, Vector3, VectorOptions> _tween;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="duration"></param>
        public void CollectByPlayer(Transform parent, float duration = 0.5f)
        {
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