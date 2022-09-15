namespace App.Domain.Notes
{

    public class NoteProperty
    {
        public float Begin;
        public NoteType Type;

        public NoteProperty(float begin, NoteType noteType)
        {
            Begin = begin;
            Type = noteType;

        }
    }
}