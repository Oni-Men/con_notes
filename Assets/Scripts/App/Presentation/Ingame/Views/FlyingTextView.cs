using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace App.Presentation.Ingame.Views
{
    public class FlyingTextView : MonoBehaviour
    {

        [SerializeField] private TMP_Text tmpText;

        private void Start()
        {
            this.transform
                .DOMoveZ(-0.5f, 1f)
                .SetEase(Ease.OutQuint)
                .OnComplete(() =>
                {
                    Destroy(gameObject);
                });
        }

        public void SetText(string str)
        {
            tmpText.text = str;
        }

        public void SetMaterial(Material material)
        {
            tmpText.fontMaterial = material;
        }
    }
}