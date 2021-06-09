using System;
using System.Collections;
using Spine;
using Spine.Unity;

using UnityEngine;

public class SpineAttackHandle : MonoBehaviour
{
    [SpineEvent] public string onAttack = "OnBullet";
    [SpineEvent] public string endAttack = "EndAttack";

    private IAnimationStateComponent _iAnimationState;
    private ISkeletonAnimation _iSkeletonAnimation;
    private ISkeletonComponent _iSkeletonComponen;

    private Action _onAttackAction;
    private Action _onEndAttackAction;

    public void Initialize(Action onAttackAction, Action onEndAttackAction)
    {
        _onAttackAction = onAttackAction;
        _onEndAttackAction = onEndAttackAction;
    }

    private void HandleEvent(TrackEntry trackentry, Spine.Event e)
    {
        if (e.Data.Name == onAttack)
        {
            _onAttackAction?.Invoke();
        }
        else if (e.Data.Name == endAttack)
        {
            _onEndAttackAction?.Invoke();
        }
    }

    private void Awake() { this.GetSpineInterfaces(out _iAnimationState, out _iSkeletonAnimation, out _); }

    private IEnumerator Start() { yield return _iAnimationState.SubscribeEvents(null, HandleEvent, null, null); }

    private void OnDestroy() { _iAnimationState.UnsubscribeEvents(null, HandleEvent, null, null); }
}