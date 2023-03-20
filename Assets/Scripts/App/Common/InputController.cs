using System;
using UniRx;
using UnityEngine;

namespace App.Common
{
    public class InputController : MonoBehaviour
    {
        public class LaneStateData
        {
            public readonly int laneId;
            public readonly bool isPressed;

            public LaneStateData(int laneId, bool isPressed)
            {
                this.laneId = laneId;
                this.isPressed = isPressed;
            }
        }
        
        private readonly Subject<LaneStateData> _laneState = new();

        public IObservable<LaneStateData> LaneState => _laneState;

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