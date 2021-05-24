using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    [SerializeField] private WorldType worldType;

    public WorldType WorldType { get => worldType; set => worldType = value; }
}
