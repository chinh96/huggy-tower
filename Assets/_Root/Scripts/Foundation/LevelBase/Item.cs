using UnityEngine;

public abstract class Item : MonoBehaviour, IUnit
{
    public GameObject ThisGameObject => gameObject;
    public EUnitState State { get; set; }
    public virtual EUnitType Type { get; } = EUnitType.Item;
    public virtual void DarknessRise() { }

    public virtual void LightReturn() { }

    public abstract void Collect(IUnit affectTarget);
}