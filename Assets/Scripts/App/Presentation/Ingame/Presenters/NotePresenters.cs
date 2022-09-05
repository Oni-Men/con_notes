using System;
using System.Collections.Generic;
using System.Linq;

namespace App.Presentation.Ingame.Presenters
{
    public class NotePresenters
    {

        private readonly List<NotePresenter> _presenters = new List<NotePresenter>();
        
        public IReadOnlyList<NotePresenter> Presenters => _presenters;

        public void Initialize()
        {
            Bind();
        }

        private void Bind()
        {
            
        }

        public NotePresenter GetNearestNotePresenter(int laneId)
        {
            //指定されたレーン内にあるノートのプレゼンターを収集
            var lanePresenters = _presenters
                .Where(presenter => presenter.LaneId == laneId)
                .ToList();

            if (!lanePresenters.Any())
            {
                return null;
            }

            //一番近いノートのプレゼンター
            var nearestNotePresenter = lanePresenters.OrderBy(presenter => Math.Abs(presenter.ZPosition)).First();
            return nearestNotePresenter;
        }
        
        public void AddNotePresenter(NotePresenter notePresenter)
        {
            _presenters.Add(notePresenter);
        }

        public void RemoveNotePresenter(NotePresenter notePresenter)
        {
            _presenters.Remove(notePresenter);
            notePresenter.Dispose();
        }
    }
}