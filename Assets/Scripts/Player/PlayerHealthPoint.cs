using System.Collections;
using UnityEngine;
using Fusion;
using System;

namespace DEMO
{
    public class PlayerHealthPoint : NetworkBehaviour, IDamagable
    {
        [SerializeField] private int maxHP;
        public int MaxHP => maxHP;

        [Networked]
        [OnChangedRender(nameof(HandleHpChanged))] 
        public int HP { get; set; }

        private Action<int> OnHpChanged = null;

        public override void Spawned()
        {
            HP = maxHP;
        }

        public void TakeDamage(int damage)
        {
            if(Object.HasStateAuthority)
                HP -= damage;
        }

        public void HandleHpChanged()
        {
            OnHpChanged?.Invoke(HP);
        }

        public void Subscribe(Action<int> action)
        {
            OnHpChanged += action;
        }

        public void Unsubscribe(Action<int> action)
        {
            OnHpChanged -= action;
        }
    }
}
