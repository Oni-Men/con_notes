using System;
using System.IO;
using App.Common;
using App.Domain;
using App.Domain.Ingame.Enums;
using App.Presentation.Common;
using App.Presentation.Ingame.Presenters;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;


namespace App.Presentation.Ingame.Views
{
    public class InGameViewRoot : MonoBehaviour
    {
        public struct InGameViewParam
        {
            public string songDirectoryPath; //譜面の情報が格納されたディレクトリのパス
        }

        [SerializeField] private InputController inputController;

        [SerializeField] private StatusViewRoot statusViewRoot;

        [SerializeField] private Generator noteGenerator;

        [SerializeField] private PlayableDirector playableDirector;

        [SerializeField] private ParticleSystem mapleEffectPrefab;

        [SerializeField] public FadeInOutView fadeInoutView;

        private GamePresenter _presenter;

        private readonly Subject<Unit> _endPlayingEvent = new();
        public IObservable<Unit> EndPlayingEvent => _endPlayingEvent.First();

        public async UniTask Initialize(InGameViewParam param)
        {
            _presenter = new GamePresenter(this, statusViewRoot, inputController);
            _presenter.Initialize(param);

            Time.timeScale = 1.0f;
            var mapPath = Path.Join(param.songDirectoryPath, "map");
            var playableAsset = Resources.Load<PlayableAsset>(mapPath);
            playableDirector.playableAsset = playableAsset;

            
            // 5秒後に音楽を再生する
            var waitDuration = GameManager.ShouldPlayCutIn ? 5f : 0f;
            await UniTask.Delay(TimeSpan.FromSeconds(waitDuration));
            
            playableDirector.Play();
            
            // 音楽の再生が終わったらイベントを発行する
            playableDirector.stopped += p =>
            {
                _endPlayingEvent.OnNext(Unit.Default);
            };
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

        public async UniTask PlaySlowEffect(float duration = 1.5f)
        {
            const float interval = 0.1f;
            for (var elapsed = 0f; elapsed < duration; elapsed+=interval)
            {
                Time.timeScale *= 0.9f;
                await UniTask.Delay(TimeSpan.FromSeconds(interval), DelayType.Realtime);
            }
            Time.timeScale = 1.0f; 
            playableDirector.Stop();
        }
    }
}