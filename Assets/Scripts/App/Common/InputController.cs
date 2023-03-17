using System;
using UniRx;
using UnityEngine;

namespace App.Common
{
    public class InputController : MonoBehaviour
    {
        public class LaneStateData
        {
            public readonly int LaneId;
            public readonly bool IsPressed;

            public LaneStateData(int laneId, bool isPressed)
            {
                LaneId = laneId;
                IsPressed = isPressed;
            }
        }
        
        private Subject<LaneStateData> _laneState;

        public IObservable<LaneStateData> LaneStateObserver => _laneState = new();

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                _laneState.OnNext(new LaneStateData(-1, true));
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                _laneState.OnNext(new LaneStateData(0, true));
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                _laneState.OnNext(new LaneStateData(1, true));
            }

            if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                _laneState.OnNext(new LaneStateData(-1, false));
            }

            if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                _laneState.OnNext(new LaneStateData(0, false));
            }

            if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                _laneState.OnNext(new LaneStateData(1, false));
            }
        }
    }
}