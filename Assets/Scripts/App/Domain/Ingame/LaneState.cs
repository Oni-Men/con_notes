using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace App.Domain.Ingame
{
    public class LaneState
    {
        public bool IsPressed { get; private set; }
        public float LastPressedAt { get; private set; }

        public void Press()
        {
            IsPressed = true;
            LastPressedAt = Time.realtimeSinceStartup;
        }

        public void Release()
        {
            IsPressed = false;
        }
    }
}
