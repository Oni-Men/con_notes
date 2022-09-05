using System.Linq;
using App.Application.Ingame;
using App.Common;
using App.Presentation.Ingame.Presenters;
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
        
        [SerializeField]
        private DebugView debugView;

        [SerializeField] private JudgementView judgementPrefab;
        
        [SerializeField] private Generator noteGenerator;

        [SerializeField] private ToriiGenerator toriiGenerator;
        
        [SerializeField] private PlayableDirector playableDirector;
        
        private GamePresenter _presenter;
        
        void Awake()
        {
            _presenter = new GamePresenter(this, statusViewRoot, inputController, debugView);
            _presenter.Initialize();
            
            BindBeatmap();
        }

        /*
         *  ResourcesからロードしたPlayableにSignal Receiverをバインドし、
         *  シーンに存在するタイムラインのPlayable Directorから再生できるようにしている。
         */
        private void BindBeatmap()
        {
            //TODO 今のところ一曲のみ。選曲機能を実装して、複数曲プレイできるようにする
            var beatmap = Resources.Load<PlayableAsset>("Songs/和のEDM/和のEDM");
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

        public JudgementView CreateJudgementView(JudgementViewModel viewModel)
        {
            var judgementView = Instantiate(judgementPrefab, viewModel.Position, Quaternion.identity);
            return judgementView;
        }
    }
}