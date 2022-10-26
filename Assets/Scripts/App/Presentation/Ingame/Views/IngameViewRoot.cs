using System;
using App.Common;
using App.Domain;
using App.Domain.Ingame.Enums;
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

        [SerializeField] private JudgementView judgementPrefab;

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
            Observable.Timer(TimeSpan.FromSeconds(5)).Subscribe(_ => { playableDirector.Play(); });

            // デバッグ: 15秒後にゲームを終了する
            // Observable.Timer(TimeSpan.FromSeconds(15)).Subscribe(_ =>
            // {
            //     _endPlayingEvent.OnNext(Unit.Default);
            // });


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
            if (!GameConst.JudgementText.ContainsKey(type))
            {
                return;
            }

            if (noteGenerator.JudgementAndMaterials[type] == null)
            {
                return;
            }

            // Debug.Log($"Lane: {laneId}, Type: {type}");

            var pos = new Vector3(laneId, 0, 0);
            var text = GameConst.JudgementText[type];
            var material = noteGenerator.JudgementAndMaterials[type];

            noteGenerator.SpawnFlyingText(pos, text, material);
        }

        public void StopPlayingBeatmap()
        {
            Observable
                .Interval(TimeSpan.FromSeconds(0.1))
                .Take(10)
                .Subscribe(_ => { Time.timeScale *= 0.8f; })
                .AddTo(this);
            Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe(_ =>
            {
                Time.timeScale = 1.0f;
                playableDirector.Stop();
            }).AddTo(this);
        }
    }
}