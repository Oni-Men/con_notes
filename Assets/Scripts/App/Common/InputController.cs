using System;
using System.Collections.Generic;
using UnityEngine;

namespace App.Common
{
    public class InputController : MonoBehaviour
    {
        private Dictionary<int, bool> laneState;
        private Subject<IReadOnlyDictionary<int, bool>> laneStateSubject;

        public IObservable<IReadOnlyDictionary<int, bool>> laneStateObserver => laneStateSubject;

        void Awake()
        {
            laneStateSubject = new();
            laneState = new()
            {
                { -1, false },
                { 0, false },
                { 1, false },
            };
        }

        void Update()
        {

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                laneState[-1] = true;
                laneStateSubject.OnNext(laneState);
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                laneState[0] = true;
                laneStateSubject.OnNext(laneState);
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                laneState[1] = true;
                laneStateSubject.OnNext(laneState);
            }


            if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                laneState[-1] = false;
                laneStateSubject.OnNext(laneState);
            }

            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                laneState[0] = false;
                laneStateSubject.OnNext(laneState);
            }

            if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                laneState[1] = false;
                laneStateSubject.OnNext(laneState);
            }
        }
    }
}