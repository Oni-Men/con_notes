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
            Left,
            Right,
            Center
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

            // すべての文字の位置を計算, 配置する
            // 同じタイミングで表示する文字でグループ化する
            // グループごとに遅延を設定してフェードアニメーションを適応
            var tmpGroups = new List<List<TMP_Text>> { new() };
            for (var i = 0; i < text.Length; i++)
            {
                var c = text[i];
                var nextChar = i + 1 < text.Length ? text[i + 1] : ' ';

                sb.Append(c);
                var tmpText = _tmpPool.Get();
                _charList.Add(tmpText);

                // TMP_Textの設定
                tmpText.alignment = TextAlignmentOptions.Center;
                tmpText.text = c.ToString();
                tmpText.alpha = 0;

                // Transformの調整
                tmpText.rectTransform.sizeDelta = new Vector2(tmpText.fontSize, tmpText.fontSize);
                tmpText.transform.localScale = Vector3.one;
                tmpText.rectTransform.pivot = TopRightPivot;
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
                
                // 一文字設定したら移動する
                var needLineBreak = IsLineBreakCharacter(c);
                if (y - tmpText.fontSize < rect.yMin)
                {
                    // 行頭に句読点がこないように
                    if (nextChar is '、' or '。')
                    {
                        // y += tmpText.fontSize * 0.5f;
                        needLineBreak = false;
                    }
                    else
                    {
                        needLineBreak = true;
                    }
                }

                tmpGroups.Last().Add(tmpText);
                
                // 改行するときは y を一番上に、x を一行分ずらす
                if (needLineBreak)
                {
                    y = rect.yMax;
                    x -= tmpText.fontSize * 1.5f; // 縦書きなので左にずらす
                    if (IsLineBreakCharacter(c))
                    {
                        tmpGroups.Add(new List<TMP_Text>());
                        sb.Clear();
                    }
                }
                // 改行しないときは y を一文字分ずらす
                else
                {
                    y -= tmpText.fontSize;
                }
            }

            foreach (var tmpTexts in tmpGroups)
            {
                foreach (var tmpText in tmpTexts)
                {
                    tmpText.DOFade(1f, fadeIn);
                }

                try
                {
                    var wait = tmpTexts.Count / 8f * fadeIn;
                    await UniTask.Delay(TimeSpan.FromSeconds(Mathf.Clamp(wait, 1f, 2.5f)),
                        cancellationToken: cancellationToken);
                }
                catch (OperationCanceledException _)
                {
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
            tmpText.text = "";
            tmpText.enabled = true;
            tmpText.transform.localPosition = new Vector3();
            tmpText.transform.rotation = new Quaternion();
        }

        private static void ActionOnRelease(TMP_Text tmpText)
        {
            tmpText.enabled = false;
            tmpText.text = "";
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