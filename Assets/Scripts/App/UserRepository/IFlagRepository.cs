namespace UserRepository
{
    public interface IFlagRepository
    {
        public void AddFlag(string flagId);

        public bool RemoveFlag(string flagId);

        public bool HasFlag(string flagId);
    }
}