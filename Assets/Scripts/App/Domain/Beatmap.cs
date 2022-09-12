using System.Collections.Generic;
using App.Domain.Ingame.Enums;

namespace App.Domain
{
    public class Beatmap
    {
        public string Title { get; private set; }
        public string Artist { get; private set; }
        public int BPM { get; private set; }

        private readonly List<> noteList;
        
        public Beatmap(string title, string artist, int bpm)
        {
            Title = title;
            Artist = artist;
            BPM = bpm;
        }
        
    }
}