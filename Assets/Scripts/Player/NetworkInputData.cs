using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Fusion;

namespace DEMO.Player
{
    public enum InputButtons
    {
        FIRE,
        TESTDAMAGE,
        PICKUP,
        REPAIR,
        REBLOOD
    }

    public struct NetworkInputData : INetworkInput
    {
        public NetworkButtons buttons;

        public Vector2 movementInput;
        public Vector2 mousePosition;
    }
}