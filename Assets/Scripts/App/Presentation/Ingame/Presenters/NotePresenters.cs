using System;
using System.Collections.Generic;
using System.Linq;

namespace App.Presentation.Ingame.Presenters
{
    public class NotePresenters
    {

        private readonly Dictionary<int, List<NotePresenter>> _presenters = new ();

        public NotePresenter GetNearestNotePresenter(int laneId)
        {
            //指定されたレーン内にあるノートのプレゼンターを収集
            if (!_presenters.ContainsKey(laneId))
            {
                return null;
            }
            
            var lanePresenters = _presenters[laneId];
            if (lanePresenters.Count == 0)
            {
                return null;
            }

            //一番近いノートのプレゼンター
            var nearestNotePresenter = lanePresenters.OrderBy(presenter => Math.Abs(presenter.ZPosition)).First();
            return nearestNotePresenter;
        }
        
        public void AddNotePresenter(NotePresenter notePresenter)
        {
            if (!_presenters.ContainsKey(notePresenter.LaneId))
            {
                _presenters[notePresenter.LaneId] = new List<NotePresenter>();
            }
            _presenters[notePresenter.LaneId].Add(notePresenter);
        }

        public void RemoveNotePresenter(NotePresenter notePresenter)
        {
            if (_presenters.ContainsKey(notePresenter.LaneId))
            {
                _presenters[notePresenter.LaneId].Remove(notePresenter);
            }
        }
    }
}