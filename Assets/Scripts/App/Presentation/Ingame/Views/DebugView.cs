using App.Domain;
using App.Domain.Ingame.Enums;
using TMPro;
using UnityEngine;

namespace App.Presentation.Ingame.Views
{
    public class DebugView : MonoBehaviour
    {

        [SerializeField] private TMP_Text _text;

        void LateUpdate()
        {
//             var game = gameManager.CurrentGame;
//             if (game == null)
//             {
//                 _text.text = "Not playing";
//                 return;
//             }
//
//             var miss = game.EvalCounts.ContainsKey(JudgementType.Miss) ? game.EvalCounts[JudgementType.Miss]: 0;
//             
//             _text.text = $@"
// Score: {game.Score}
// Miss: {miss}
// Combo: {game.CurrentCombo}
// Max Combo: {game.MaxCombo}";
        }
    }
}
