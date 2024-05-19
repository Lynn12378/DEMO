using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Fusion;
using System;

namespace DEMO
{
    public class EnemyHealthPoint : NetworkBehaviour
    {
        [SerializeField] public Slider healthPointSlider;

        [Networked]
        [OnChangedRender(nameof(HandleHpChanged))]
        public int Hp { get; set; }

        public void HandleHpChanged()
        {
            healthPointSlider.value = Hp;
        }
    }
}
