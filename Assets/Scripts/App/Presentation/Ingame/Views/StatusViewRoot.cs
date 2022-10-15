using DG.Tweening;
using TMPro;
using UnityEngine;

namespace App.Presentation.Ingame.Views
{
    public class StatusViewRoot : MonoBehaviour
    {

        [SerializeField] private TMP_Text comboText;
        [SerializeField] private TMP_Text scoreText;

        public void UpdateCombo(int combo)
        {
            comboText.text = $"×{combo.ToString()}";
            comboText.rectTransform.localScale = Vector3.one * 1.05f;
            comboText.rectTransform
                .DOScale(1.0f, 0.3f)
                .SetEase(Ease.OutQuint);
        }

        public void UpdateMaxCombo(int maxCombo)
        {
            // maxComboText.text = $"最大コンボ: {maxCombo.ToString()}";
        }

        public void UpdateScore(int score)
        {
            scoreText.text = $"{score.ToString()}点";
            scoreText.rectTransform.localScale = Vector3.one * 1.05f;
            scoreText.rectTransform
                .DOScale(1.0f, 0.3f)
                .SetEase(Ease.OutQuint);
        }
    }
}
