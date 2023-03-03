using System;
using App.Common;
using App.Domain;
using App.Domain.Ingame.Enums;
using App.Presentation.Common;
using App.Presentation.Ingame.Presenters;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;


namespace App.Presentation.Ingame.Views
{
    public class IngameViewRoot : MonoBehaviour
    {
        [SerializeField] private InputController inputController;

        [SerializeField] private StatusViewRoot statusViewRoot;

        [SerializeField] private Generator noteGenerator;

        [SerializeField] private PlayableDirector playableDirector;

        [SerializeField] private ParticleSystem mapleEffectPrefab;

        [SerializeField] public FadeInOutView fadeInoutView;

        private GamePresenter _presenter;

        private readonly Subject<Unit> _endPlayingEvent = new();
        public IObservable<Unit> EndPlayingEvent => _endPlayingEvent;

        void Awake()
        {
            _presenter = new GamePresenter(this, statusViewRoot, inputController);
            _presenter.Initialize();

            Time.timeScale = 1.0f;
        }

        void Start()
        {
            // 5秒後に音楽を再生する
            var waitDuration = GameManager.ShouldPlayCutIn ? 5f : 0f;
            Observable.Timer(TimeSpan.FromSeconds(waitDuration)).Subscribe(_ => { playableDirector.Play(); });

            // 音楽の再生が終わったらイベントを発行する
            playableDirector.stopped += p => { _endPlayingEvent.OnNext(Unit.Default); };
        }

        public void SpawnParticle(int laneId, float amount)
        {
            var pos = new Vector3(laneId, 0, 0);
            Instantiate(mapleEffectPrefab, pos, Quaternion.identity);
        }

        public void SpawnFlyingText(int laneId, JudgementType type)
        {
            if (!GameConst.EvalNames.ContainsKey(type))
            {
                return;
            }

            if (noteGenerator.JudgementAndMaterials[type] == null)
            {
                return;
            }

            // Debug.Log($"Lane: {laneId}, Type: {type}");

            var pos = new Vector3(laneId, 0, 0);
            var text = GameConst.EvalNames[type];
            var material = noteGenerator.JudgementAndMaterials[type];

            noteGenerator.SpawnFlyingText(pos, text, material);
        }

        public IObservable<long> PlaySlowEffect()
        {
            const float interval = 0.1f;
            const int times = 15;

            Observable
                .Interval(TimeSpan.FromSeconds(interval), Scheduler.MainThreadIgnoreTimeScale)
                .Take(times)
                .Subscribe(_ => { Time.timeScale *= 0.9f; })
                .AddTo(this);
            var timer = Observable.Timer(TimeSpan.FromSeconds(interval * times), Scheduler.MainThreadIgnoreTimeScale);
            timer.Subscribe(_ =>
            {
                Time.timeScale = 1.0f;
                playableDirector.Stop();
            }).AddTo(this);

            return timer;
        }
    }
}