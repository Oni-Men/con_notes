namespace App.Domain.Notes
{
    public class NormalNoteProperty : NoteProperty
    {
        public NormalNoteProperty(float begin) : base(begin, NoteType.Single)
        {
        }
    }
}
