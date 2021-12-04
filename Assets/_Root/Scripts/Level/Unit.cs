using System;
using TMPro;
using UnityEngine;
using DG.Tweening;

public abstract class Unit : MonoBehaviour, IUnit, IAttack
{
    [SerializeField] protected EUnitState state;
    [SerializeField] protected TextMeshProUGUI txtDamage;
    [SerializeField] protected int damage;

    private void Awake()
    {
        if (txtDamage != null && damage > 0)
        {
            if (this as EnemyGoblin || this as EnemyKappa)
            {
                txtDamage.DOCounter(-damage, -damage, 0);
            }
            else
            {
                txtDamage.DOCounter(damage, damage, 0);
            }
        }

        if (!TGDatas.IsInTG)
        {
            var turkey = GetComponentInChildren<TGTurkey>();
            if (turkey != null)
            {
                Destroy(turkey.gameObject);
            }
        }
    }

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

    public void CheckTurkey()
    {
        var turkey = GetComponentInChildren<TGTurkey>();
        if (turkey != null)
        {
            turkey.Move();
        }
    }
}