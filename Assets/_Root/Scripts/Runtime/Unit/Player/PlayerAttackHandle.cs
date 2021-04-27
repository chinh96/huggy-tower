using System;
using System.Collections;
using Lance.Common;
using Spine;
using Spine.Unity;

namespace Lance.TowerWar.Unit
{
    using UnityEngine;

    public class PlayerAttackHandle : MonoBehaviour
    {
        [SpineEvent] public string onAttack = "OnBullet";

        private IAnimationStateComponent _iAnimationState;
        private ISkeletonAnimation _iSkeletonAnimation;
        private ISkeletonComponent _iSkeletonComponen;

        private Action _onAttackAction;
        public void Initialize(Action onAttackAction) { _onAttackAction = onAttackAction; }

        private void HandleEvent(TrackEntry trackentry, Spine.Event e)
        {
            if (e.Data.Name == onAttack)
            {
                _onAttackAction?.Invoke();
            }
        }

        private void Awake() { this.GetSpineInterfaces(out _iAnimationState, out _iSkeletonAnimation, out _); }

        private IEnumerator Start() { yield return _iAnimationState.SubscribeEvents(null, HandleEvent, null, null); }

        private void OnDestroy() { _iAnimationState.UnsubscribeEvents(null, HandleEvent, null, null); }
    }
}