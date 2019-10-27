using System;
using System.Threading.Tasks;

namespace Core
{
    public interface ISnapshotStore
    {

        Task<Snapshot> GetSnapshotAsync<T>(Guid aggregateId) where T: Aggregate;
        Task<long> SaveSnapshotAsync<T>(Snapshot snapshot) where T:Aggregate;
    }
}