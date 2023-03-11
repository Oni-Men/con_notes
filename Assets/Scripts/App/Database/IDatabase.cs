using System.Collections.Generic;

namespace Database
{
    public interface IDatabase<T>
    {
        public IReadOnlyDictionary<string, T> All();
        public void Put(string flagId, T b);
        public void Delete(string flagId);
        public bool Has(string flagId);
    }
}