using System;
using System.Linq;
using App.Common;
using App.Domain;
using App.Domain.Ingame.Enums;
using App.Presentation.Ingame.Presenters;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace App.Presentation.Ingame.Views
{
    public class IngameViewRoot : MonoBehaviour
    {
        [SerializeField]
        private InputController inputController;

        [SerializeField]
        private StatusViewRoot statusViewRoot;
        
        [SerializeField] private JudgementView judgementPrefab;
        
        [SerializeField] private Generator noteGenerator;

        [SerializeField] private ToriiGenerator toriiGenerator;
        
        [SerializeField] private PlayableDirector playableDirector;

        [SerializeField] private ParticleSystem mapleEffectPrefab;
        
        private GamePresenter _presenter;

        private readonly Subject<Unit> _endPlayingEvent = new();
        public IObservable<Unit> EndPlayingEvent => _endPlayingEvent;

        void Awake()
        {
            _presenter = new GamePresenter(this, statusViewRoot, inputController);
            _presenter.Initialize();
            
            //BindBeatmap();

        }

        void Start()
        {
            // 5秒後に音楽を再生する
            Observable.Timer(TimeSpan.FromSeconds(5)).Subscribe(_ =>
            {
                playableDirector.Play();
            });

            // 音楽の再生が終わったらイベントを発行する
            playableDirector.stopped += p =>
            {
                _endPlayingEvent.OnNext(Unit.Default);
            };
        }

        /*
         *  ResourcesからロードしたPlayableにSignal Receiverをバインドし、
         *  シーンに存在するタイムラインのPlayable Directorから再生できるようにしている。
         */
        private void BindBeatmap()
        {
            //TODO 今のところ一曲のみ。選曲機能を実装して、複数曲プレイできるようにする
            var beatmap = Resources.Load<PlayableAsset>("Songs/紅葉の花束/map");
            var signalBindings = beatmap.outputs.Where(binding => binding.sourceObject is SignalTrack);
            foreach (var binding in signalBindings)
            {
                if (binding.streamName.Equals("Signal Torii"))
                {
                    playableDirector.SetGenericBinding(binding.sourceObject, toriiGenerator.GetComponent<SignalReceiver>());
                }
                else
                {
                    playableDirector.SetGenericBinding(binding.sourceObject, noteGenerator.GetComponent<SignalReceiver>());
                }
                
            }


            playableDirector.playableAsset = beatmap;
            playableDirector.Play();
            
        }

        public void SpawnParticle(int laneId, float amount) {
            var pos = new Vector3(laneId, 0, 0);
            Instantiate(mapleEffectPrefab, pos, Quaternion.identity);
        }

        public void SpawnFlyingText(int laneId, JudgementType type)
        {
            if (!GameConst.JudgementText.ContainsKey(type))
            {
                return;
            }
            
            if (noteGenerator.JudgementAndMaterials[type] == null) {
                return;
            }
            
            Debug.Log($"Lane: {laneId}, Type: {type}");
            
            var pos = new Vector3(laneId, 0, 0);
            var text = GameConst.JudgementText[type];
            var material = noteGenerator.JudgementAndMaterials[type];
            
            noteGenerator.SpawnFlyingText(pos, text, material);
        }
    }
}