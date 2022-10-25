using System;
using System.Collections.Generic;
using App.Domain;
using App.Domain.Ingame.Enums;
using App.Presentation.Ingame.Views;
using UnityEngine;

namespace App.Common
{
    public class Generator : MonoBehaviour
    {
        [SerializeField] private NoteView prefab;

        [SerializeField] private ToriiView toriiPrefab;

        [SerializeField] private FlyingTextView flyingText;

        [SerializeField] private Material perfectMaterial;
        [SerializeField] private Material goodMaterial;
        [SerializeField] private Material badMaterial;
        [SerializeField] private Material missMaterial;

        private Dictionary<JudgementType, Material> _judgementAndMaterials;

        public IReadOnlyDictionary<JudgementType, Material> JudgementAndMaterials => _judgementAndMaterials;

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

        public void SpawnNote(int laneId)
        {
            var x = laneId * GameConst.NoteWidth;
            var z = prefab.Speed * GameConst.Lifetime;
            var position = new Vector3(x, 0.01f, z);
            Instantiate(prefab, position, Quaternion.identity);
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