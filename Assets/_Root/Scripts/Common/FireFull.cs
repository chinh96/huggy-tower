using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireFull : MonoBehaviour
{
    public ParticleSystem Fx;
    private bool isRotate;

    private void Update()
    {
        if (isRotate)
        {
            transform.Rotate(Vector3.up * 30 * Time.deltaTime, Space.Self);
        }
    }

    public void Show()
    {
        Fx.Play();
        isRotate = true;
    }
}
