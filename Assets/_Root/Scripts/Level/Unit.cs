using System;
using TMPro;
using UnityEngine;

public abstract class Unit : MonoBehaviour, IUnit, IAttack
{
    [SerializeField] protected EUnitState state;
    [SerializeField] protected TextMeshProUGUI txtDamage;
    [SerializeField] protected int damage;

    public GameObject ThisGameObject => gameObject;

    public EUnitState State { get => state; set => state = value; }

    public abstract EUnitType Type { get; protected set; }

    public int Damage { get => damage; set => damage = value; }

    public TextMeshProUGUI TxtDamage => txtDamage;

    public abstract void OnBeingAttacked();

    public abstract void OnAttack(int damage, Action callback);

    public abstract void DarknessRise();

    public abstract void LightReturn();

    public Color ColorBlood;
}