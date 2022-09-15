using System;
using UnityEngine;

namespace App.Common
{

    public class KeyBinding
    {
        private KeyCode _keyCode;

        private GameAction _gameAction;

        public KeyBinding(KeyCode keyCode, GameAction gameAction)
        {
            _keyCode = keyCode;
            _gameAction = gameAction;
        }
    }

}