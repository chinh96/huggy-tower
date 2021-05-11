using System.Collections.Generic;
using System.Linq;
using MEC;

namespace Lance.TowerWar.Unit
{
    using UnityEngine;

    public class GemHandles : MonoBehaviour
    {
        public Gems[] gems;

        private bool _flagCollectGem;
        private CoroutineHandle _collectGemHandle;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="duration"></param>
        /// <param name="durationIncreasePerGem"></param>
        public void StartCollectGem(Transform root, float duration, float durationIncreasePerGem)
        {
            if (!_flagCollectGem)
            {
                _flagCollectGem = true;
                _collectGemHandle = IeStartCollectGem(root, duration, durationIncreasePerGem).RunCoroutine();
            }
        }

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
    }
}