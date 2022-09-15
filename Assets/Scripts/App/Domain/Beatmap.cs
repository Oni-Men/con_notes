using System;
using System.Collections.Generic;
using App.Domain.Ingame.Enums;
using App.Domain.Notes;

namespace App.Domain
{
    public class Beatmap
    {
        public readonly string Title;
        public readonly string Artist;
        public readonly int BPM;
        public readonly int Difficulty;
        private List<NoteProperty> noteList;
        
        public IReadOnlyList<NoteProperty> NoteList => noteList;

        public Beatmap(string title, string artist = "Unkown", int bpm = 130, int difficulty = 1)
        {
            Title = title;
            Artist = artist;
            BPM = bpm;
            Difficulty = difficulty;

            if (BPM < 1 || BPM > 5)
            {
                throw new ArgumentException("BPM should be inside the range 1 to 5.");
            }
        }
        
    }
}