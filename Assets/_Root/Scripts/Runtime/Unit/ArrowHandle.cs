using System;
using Lance.TowerWar.Controller;

namespace Lance.TowerWar.Unit
{
    using UnityEngine;

    public class ArrowHandle : MonoBehaviour
    {
        private Action<GameObject> _callbackAttackPlayer;

        public void Initialize(Action<GameObject> callbackAttackPlayer) { _callbackAttackPlayer = callbackAttackPlayer; }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Enemy"))
            {
                return;
            }

            if (other.CompareTag("Player"))
            {
                var player = other.GetComponentInParent<Player>();

                if (player != null && player.State != EUnitState.Invalid)
                {
                    _callbackAttackPlayer?.Invoke(gameObject);
                }

                return;
            }
        }
    }
}