using App.Domain;
using UnityEngine;

namespace App.Common
{
    public class InputController : MonoBehaviour
    {
        void Update()
        {
            var gameManager = GameManager.GetInstance();
            var game = gameManager.CurrentGame;

            if (game == null)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                game.DoJudge(-1);   
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                game.DoJudge(0);
            }
            
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                game.DoJudge(1);
            }
        }
    }
}