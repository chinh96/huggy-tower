using System;
using TMPro;

public interface IAttack
{
    int Damage { get; set; }
    TextMeshProUGUI TxtDamage { get; }
    void OnBeingAttacked();
    void OnAttack(int damage, Action callback);
}