using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

namespace App.Presentation.Scenario
{
    public class VerticalTextView : MonoBehaviour
    {
        public enum TextAlign
        {
            Left, Right, Center
        }

        private static readonly Vector2 CenterPivot = new Vector2(0.5f, 0.5f);
        private static readonly Vector2 TopRightPivot = new Vector2(1f, 1f);
        private static readonly Vector2 PunctuationPivot = new Vector2(0.4f, 0.4f);

        [SerializeField]
        public RectTransform rectTransform;

        [SerializeField]
        public Transform offsetRoot;
        
        [SerializeField]
        private CanvasGroup canvasGroup;

        [SerializeField]
        private Color vertexColor;

        [SerializeField]
        private Material fontMaterial;

        [SerializeField]
        [TextArea(5, 10)]
        private string text;

        [SerializeField]
        public TextAlign textAlign;
        
        public string Text => text;
        
        [SerializeField]
        private int fontSize;

        private int _textHash = -1;
        private float _width;

        [SerializeField]
        private TMP_FontAsset fontAsset;

        private int _poolNextId = 0;
        private ObjectPool<TextMeshProUGUI> _tmpPool;

        private List<TextMeshProUGUI> _charList;

        private void Awake()
        {
            _tmpPool = new ObjectPool<TextMeshProUGUI>(
                CreateFunc,
                ActionOnGet,
                ActionOnRelease,
                ActionOnDestroy
            );
            _charList = new List<TextMeshProUGUI>();
        }

        private void Update()
        {
            text ??= "";
            var hash = text.GetHashCode();
            if (hash == _textHash) return;
            _textHash = hash;
            SetTextAsync(text, gameObject.GetCancellationTokenOnDestroy()).Forget();
        }

        private void ClearCharList()
        {
            _charList.ForEach(_tmpPool.Release);
            _charList.Clear();
        }

        public void ClearText()
        {
            text = "";
            _textHash = text.GetHashCode();
            ClearCharList();
        }

        public async UniTask SetTextAsync(string newText, CancellationToken cancellationToken, float fadeIn = 0f)
        {
            await UpdateText(newText, cancellationToken, fadeIn);
        }

        private async UniTask UpdateText(string newText, CancellationToken cancellationToken, float fadeIn)
        {
            text = newText;
            _textHash = text.GetHashCode();
            ClearCharList();

            var rect = rectTransform.rect;
            var x = rect.xMax;
            var y = rect.yMax;
            var sb = new StringBuilder();

            foreach (var c in text)
            {
                sb.Append(c);
                var tmpText = _tmpPool.Get();

                tmpText.alignment = TextAlignmentOptions.Center;
                tmpText.rectTransform.sizeDelta = new Vector2(tmpText.fontSize, tmpText.fontSize);
                tmpText.text = c.ToString();
                tmpText.transform.localScale = Vector3.one;
                tmpText.rectTransform.pivot = TopRightPivot;

                if (cancellationToken.IsCancellationRequested)
                {
                    tmpText.alpha = 1;
                }
                else
                {
                    tmpText.alpha = 0;
                    tmpText.DOFade(1.0f, fadeIn).ToUniTask(cancellationToken: cancellationToken).Forget();
                }

                switch (c)
                {
                    case '、' or '。':
                        tmpText.rectTransform.pivot = PunctuationPivot;
                        tmpText.transform.localPosition = new Vector3(x, y, 0);
                        break;
                    case 'ー' or '-' or '…':
                        tmpText.rectTransform.pivot = CenterPivot;
                        tmpText.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, -90f));
                        tmpText.transform.localPosition =
                            new Vector3(x - tmpText.fontSize / 2, y - tmpText.fontSize / 2, 0);
                        break;
                    case '「' or '」':
                        tmpText.rectTransform.pivot = CenterPivot;
                        tmpText.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, -90f));
                        tmpText.transform.localPosition =
                            new Vector3(x - tmpText.fontSize / 2, y - tmpText.fontSize / 2, 0);
                        break;
                    default:
                        tmpText.transform.localPosition = new Vector3(x, y, 0);
                        break;
                }

                _charList.Add(tmpText);

                var needLineBreak = IsLineBreakCharacter(c);
                if (y - tmpText.fontSize < rect.yMin)
                {
                    if (c is '、' or '。')
                    {
                        y += tmpText.fontSize * 0.25f;
                        needLineBreak = false;
                    }
                    else
                    {
                        needLineBreak = true;
                    }
                }

                // 改行するときは y を一番上に、x を一行分ずらす
                if (needLineBreak)
                {
                    y = rect.yMax;
                    x -= tmpText.fontSize * 1.5f; // 縦書きなので左にずらす
                    if (!IsLineBreakCharacter(c))
                    {
                        continue;
                    }
                    var line = sb.ToString().Trim(' ', '\n', '\t');
                    sb.Clear();
                    
                    if (line.Length != 0)
                    { 
                        var sleepTime = Mathf.Clamp(0.2f * line.Length, 1.5f, 2.5f);
                        await UniTask.Delay(TimeSpan.FromSeconds(sleepTime), cancellationToken: cancellationToken);
                    }
                }
                // 改行しないときは y を一文字分ずらす
                else
                {
                    y -= tmpText.fontSize;
                }
            }
        }

        public async UniTask FadeIn(float duration)
        {
            canvasGroup.alpha = 0f;
            gameObject.SetActive(true);
            await canvasGroup.DOFade(1f, duration);
        }
        
        public async UniTask FadeOut(float duration)
        {
            canvasGroup.alpha = 1f;
            await canvasGroup.DOFade(0f, duration);
            gameObject.SetActive(false);
        }
        
        private void OnDisable()
        {
            _textHash = -1;
        }

        private TextMeshProUGUI CreateFunc()
        {
            var tmpObject = new GameObject($"VerticalText ({_poolNextId})");
            tmpObject.transform.SetParent(offsetRoot);
            _poolNextId++;

            var tmpText = tmpObject.AddComponent<TextMeshProUGUI>();
            tmpText.font = fontAsset;
            tmpText.fontSize = fontSize;
            tmpText.fontMaterial = fontMaterial;
            tmpText.color = vertexColor;
            return tmpText;
        }

        private static void ActionOnGet(TMP_Text tmpText)
        {
            tmpText.enabled = true;
            tmpText.transform.localPosition = new Vector3();
            tmpText.transform.rotation = new Quaternion();
        }

        private static void ActionOnRelease(TMP_Text tmpText)
        {
            tmpText.enabled = false;
        }

        private static void ActionOnDestroy(TMP_Text tmpText)
        {
            Destroy(tmpText.gameObject);
        }

        private static bool IsLineBreakCharacter(char c)
        {
            return c == '\n';
        }
    }
}