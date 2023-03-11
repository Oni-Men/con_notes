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
        
        
        private async void Start()
        {
            var database = DatabaseFactory.ScenarioDatabase;
            _titles = database.All().Keys.ToList();
            await InitList(gameObject.GetCancellationTokenOnDestroy());
        }

        private VerticalTextView CreateTextView()
        {
            var verticalTextView = Instantiate(original, rectTransform);
            verticalTextView.transform.localScale = Vector3.one;
            verticalTextView.transform.localPosition = Vector3.zero;
            verticalTextView.transform.localRotation = Quaternion.identity;
            verticalTextView.textAlign = VerticalTextView.TextAlign.Center;
            verticalTextView.ClearText();
            textViews.Add(verticalTextView);
            return verticalTextView;
        }

        private async UniTask InitList(CancellationToken ct)
        {
            var page = selectIndex / maxTitlesPerPage;
            if (page == _currentPage)
            {
                return;
            }

            _currentPage = page;
            var indexFirst = _currentPage * maxTitlesPerPage;
            for (var i = 0; i < maxTitlesPerPage; i++)
            {
                CreateTextView();
            }

            await UniTask.DelayFrame(30, cancellationToken:ct); // Wait for a frame to calculate rect size by layout

            for (var i = 0; i < maxTitlesPerPage; i++)
            {
                var index = indexFirst + i;
                if (index < _titles.Count)
                {
                    await textViews[index].SetTextAsync(_titles[index], ct);
                }
            } 
            
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                selectIndex++;
                InitList(gameObject.GetCancellationTokenOnDestroy()).Forget();
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                selectIndex--;
                InitList(gameObject.GetCancellationTokenOnDestroy()).Forget();
            }

            selectIndex = Math.Clamp(selectIndex, 0, _titles.Count - 1);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _scenarioSelectEvent.OnNext(_titles[selectIndex]);
            }
        }
    }
}