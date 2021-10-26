using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuckySpinHinge : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        SoundController.Instance.PlayOnce(SoundType.LuckySpinHit);
    }
}
