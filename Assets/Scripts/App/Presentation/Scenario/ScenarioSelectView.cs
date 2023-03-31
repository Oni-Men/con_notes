using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Database;
using UniRx;
using UnityEngine;

namespace App.Presentation.Scenario
{
    public class ScenarioSelectView : MonoBehaviour
    {
        [SerializeField]
        private int selectIndex = 0;

        [SerializeField]
        private int maxTitlesPerPage = 5;

        [SerializeField]
        private VerticalTextView original;

        [SerializeField]
        private List<VerticalTextView> textViews = default;

        [SerializeField]
        private RectTransform rectTransform;

        private List<string> _titles;
        private int _currentPage = -999;

        public int SelectIndex => selectIndex;

        private readonly Subject<string> _scenarioSelectEvent = new ();
        public IObservable<string> ScenarioSelectEvent => _scenarioSelectEvent;

        private void Start()
        {
            var database = DatabaseFactory.ScenarioDatabase;
            _titles = database.All().Keys.ToList();
            InitList();
            UpdateList(gameObject.GetCancellationTokenOnDestroy()).Forget();
        }

        private void CreateTextView()
        {
            var verticalTextView = Instantiate(original, rectTransform);
            var textTransform = verticalTextView.transform;
            textTransform.localScale = Vector3.one;
            textTransform.localPosition = Vector3.zero;
            textTransform.localRotation = Quaternion.identity;
            verticalTextView.textAlign = VerticalTextView.TextAlign.Center;
            verticalTextView.ClearText();
            textViews.Add(verticalTextView);
        }

        private void InitList()
        {
            for (var i = 0; i < maxTitlesPerPage; i++)
            {
                CreateTextView();
            }
        }

        private async UniTask UpdateList(CancellationToken ct)
        {
            _currentPage = selectIndex / maxTitlesPerPage;
            var indexFirst = _currentPage * maxTitlesPerPage;
            await UniTask.DelayFrame(30, cancellationToken:ct); // Wait for a frame to calculate rect size by layout

            for (var i = 0; i < maxTitlesPerPage; i++)
            {
                var index = indexFirst + i;
                var text = "";
                if (index < _titles.Count)
                {
                    text = $"{(index == selectIndex ? "*" : " ")} {_titles[index]}";
                }
                await textViews[index].SetTextAsync(text, ct);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                selectIndex++;
                UpdateList(gameObject.GetCancellationTokenOnDestroy()).Forget();
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                selectIndex--;
                UpdateList(gameObject.GetCancellationTokenOnDestroy()).Forget();
            }

            selectIndex = Math.Clamp(selectIndex, 0, _titles.Count - 1);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _scenarioSelectEvent.OnNext(_titles[selectIndex]);
            }
        }
    }
}