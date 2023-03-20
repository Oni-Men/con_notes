using System.Collections.Generic;
using App.Domain;
using App.Domain.Ingame.Enums;
using App.Presentation.Ingame.Presenters;
using App.Presentation.Ingame.Views;
using UnityEngine;

namespace App.Common
{
    public class Generator : MonoBehaviour
    {
        [SerializeField]
        private NoteViewPool notesPool;

        [SerializeField]
        private ToriiView toriiPrefab;

        [SerializeField]
        private FlyingTextView flyingText;

        [SerializeField]
        private Material perfectMaterial;

        [SerializeField]
        private Material goodMaterial;

        [SerializeField]
        private Material badMaterial;

        [SerializeField]
        private Material missMaterial;

        private Dictionary<JudgementType, Material> _judgementAndMaterials;

        public IReadOnlyDictionary<JudgementType, Material> JudgementAndMaterials => _judgementAndMaterials;

        private GamePresenter _gamePresenter;
        
        public void Initialize(GamePresenter gamePresenter)
        {
            _gamePresenter = gamePresenter;
            
            notesPool.Initialize();
        }
        
        private void Awake()
        {
            _judgementAndMaterials = new Dictionary<JudgementType, Material>()
            {
                { JudgementType.Perfect, perfectMaterial },
                { JudgementType.Good, goodMaterial },
                { JudgementType.Bad, badMaterial },
                { JudgementType.Miss, missMaterial },
            };
        }

        /// <summary>
        /// Midi Signal Receiver からのシグナルをここで受け取る
        /// </summary>
        /// <param name="laneId"></param>
        public void SpawnNote(int laneId)
        {
            _gamePresenter.SpawnNote(laneId);
        }

        public void SpawnFlyingText(Vector3 position, string text, Material material)
        {
            FlyingTextView view = Instantiate(flyingText, position, Quaternion.identity);
            view.SetText(text);
            view.SetMaterial(material);
        }

        public void SpawnGate()
        {
            var z = toriiPrefab.Speed * GameConst.Lifetime;
            Instantiate(toriiPrefab, new Vector3(0, 0, z), Quaternion.identity);
        }
    }
}