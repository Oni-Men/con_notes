using Database;

namespace UserRepository.Impl
{
    public class FlagRepository : IFlagRepository
    {
        private IDatabase<bool> _flagDatabase;
        
        public void AddFlag(string flagId)
        {
            _flagDatabase.Put(flagId, true);
        }

        public bool RemoveFlag(string flagId)
        {
            if (!HasFlag(flagId))
            {
                return false;
            }

            _flagDatabase.Delete(flagId);
            return true;
        }

        public bool HasFlag(string flagId)
        {
            return _flagDatabase.Has(flagId);
        }
    }
}