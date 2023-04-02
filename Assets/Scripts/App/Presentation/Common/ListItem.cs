using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace App.Presentation.Common
{
    public class ListItem : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text tmpText;

        [SerializeField]
        private Button button;
        
        public void SetText(string text)
        {
            tmpText.text = text;
        }

        public IObservable<Unit> AsObservable()
        {
            return button.OnClickAsObservable();
        }
    }
}