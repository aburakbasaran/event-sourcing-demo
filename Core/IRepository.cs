using System;
using System.Threading.Tasks;

namespace Core
{
    public interface IRepository
    {
        Task<T> GetByIdAsync<T>(Guid id) where T : Aggregate,new();
        Task SaveAsync<T>(T aggregate) where T : Aggregate;
    }
}