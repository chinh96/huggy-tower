using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireFull : MonoBehaviour
{
    public ParticleSystem Fx;

    public void Show()
    {
        Fx.Play();
    }
}
