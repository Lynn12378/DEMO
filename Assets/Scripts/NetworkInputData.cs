using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public enum InputButtons//�ۭq�q���s
{
    FIRE,
    SPACE
}

public struct NetworkInputData : INetworkInput
{    
    public NetworkButtons buttons;

    public Vector2 movementInput;
    public Vector2 mousePosition;
}