using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace App.Presentation.Ingame.Views
{
    public class StatusViewRoot : MonoBehaviour
    {

        [SerializeField] private TMP_Text comboText;
        [SerializeField] private TMP_Text scoreText;

        [SerializeField] private Slider healthSlider;
        
        public void UpdateCombo(int combo)
        {
            comboText.text = $"×{combo.ToString()}";
            comboText.rectTransform.localScale = Vector3.one * 1.05f;
            comboText.rectTransform
                .DOScale(1.0f, 0.3f)
                .SetEase(Ease.OutQuint);
        }
        
        public void UpdateScore(int score)
        {
            scoreText.text = $"{score.ToString()}点";
            scoreText.rectTransform.localScale = Vector3.one * 1.05f;
            scoreText.rectTransform
                .DOScale(1.0f, 0.3f)
                .SetEase(Ease.OutQuint);
        }

        public void UpdateSlider(float health)
        {
            healthSlider.DOValue(health, 0.1f).SetEase(Ease.InOutCirc);
        }
    }
}
