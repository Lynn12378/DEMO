using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Fusion;
using System;

public class HealthPoint : NetworkBehaviour
{
    [SerializeField] public Slider networkHealthSlider;

    [Networked]
    [OnChangedRender(nameof(HandleHpChanged))]
    public int Hp { get; set; }

    public void HandleHpChanged(/*NetworkBehaviourBuffer previous*/)
    {
        //var prevValue = GetPropertyReader<int>(nameof(Hp)).Read(previous);
        //Debug.Log($"Health changed: {Hp}, prev: {prevValue}");

        networkHealthSlider.value = Hp;
    }
}
