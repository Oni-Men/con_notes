using App.Domain.Ingame.Enums;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace App.Presentation.Ingame.Views
{
    public class JudgementView : MonoBehaviour
    {
        [SerializeField] private TMP_Text judgementText;

        private void Start()
        {
        }

        public async UniTask ShowJudgementAsync(JudgementType judgementType)
        {
            judgementText.text = judgementType.ToString();
            await UniTask.Delay(120);
            Destroy(gameObject);
        }
    }
}