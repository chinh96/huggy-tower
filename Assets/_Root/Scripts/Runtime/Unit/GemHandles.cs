using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class GemHandles : Item
{
    public float duration;
    public float durationIncreasePerGem;

    public Gems[] gems;

    private bool _flagCollectGem;

    public override EUnitType Type { get; } = EUnitType.Gem;

    private void IeStartCollectGem(Transform root, float duration, float durationIncreasePerGem)
    {
        var tempCacheGems = gems.Where(_ => _.gameObject.activeSelf).ToArray();
        for (int i = 0; i < tempCacheGems.Length; i++)
        {
            gems[i].CollectByPlayer(root, duration + durationIncreasePerGem * i);
        }
    }

    public void Dispose()
    {
        _flagCollectGem = false;

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
            IeStartCollectGem(affectTarget.ThisGameObject.transform, duration, durationIncreasePerGem);
        }
    }
}