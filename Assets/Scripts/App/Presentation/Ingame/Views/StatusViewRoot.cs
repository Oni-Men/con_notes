using TMPro;
using UnityEngine;

namespace App.Presentation.Ingame.Views
{
    public class StatusViewRoot : MonoBehaviour
    {

        [SerializeField] private TMP_Text comboText;
        [SerializeField] private TMP_Text maxComboText;
        [SerializeField] private TMP_Text scoreText;

        public void UpdateCombo(int combo)
        {
            comboText.text = $"コンボ: {combo.ToString()}";
        }

        public void UpdateMaxCombo(int maxCombo)
        {
            maxComboText.text = $"最大コンボ: {maxCombo.ToString()}";
        }

        public void UpdateScore(int score)
        {
            scoreText.text = $"スコア: {score.ToString()}";
        }
    }
}
