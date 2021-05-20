using System.Collections.Generic;
using System.Linq;
using MEC;

using UnityEngine;

public class GemHandles : Item
{
    public float duration;
    public float durationIncreasePerGem;

    public Gems[] gems;

    private bool _flagCollectGem;
    private CoroutineHandle _collectGemHandle;

    public override EUnitType Type { get; } = EUnitType.Gem;

    private IEnumerator<float> IeStartCollectGem(Transform root, float duration, float durationIncreasePerGem)
    {
        var tempCacheGems = gems.Where(_ => _.gameObject.activeSelf).ToArray();
        for (int i = 0; i < tempCacheGems.Length; i++)
        {
            gems[i].CollectByPlayer(root, duration + durationIncreasePerGem * i);

            yield return Timing.WaitForOneFrame;
        }
    }

    public void Dispose()
    {
        _flagCollectGem = false;
        Timing.KillCoroutines(_collectGemHandle);

        foreach (var gem in gems)
        {
            if (gem != null) gem.Dispose();
        }
    }

    public override void Collect(IUnit affectTarget)
    {
        if (!_flagCollectGem)
        {
            _flagCollectGem = true;
            _collectGemHandle = IeStartCollectGem(affectTarget.ThisGameObject.transform, duration, durationIncreasePerGem).RunCoroutine();
        }
    }
}